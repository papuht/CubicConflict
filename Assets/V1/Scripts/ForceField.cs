using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ForceField : NetworkBehaviour
{

    /**
    * Saving of the ms on enter is a good idea but we cant save it into this script,
    * since it will use that value with other shapes aswell
    *
    * New storage in player resources allows us to temporarily save simple values with a key <string key, string value>
    * Now we can set and get the pre river ms with this key and it will be shape spesific!
    * 
    * This is a good way to save some random data that we dont want permanent gets and sets for (like this one)
    */
    const string storageKey = "pre-field-damage";

    void Update() {
        Debug.Log("DDD");
    }


    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(!this.isServer) {
            return;
        }
        if (collider.gameObject.tag != "Player")
        {
            return;
        }

        CMDResetCD(collider.gameObject, this.gameObject);
        

    }

    [Server]
    public void CMDResetCD(GameObject gm, GameObject buff) {
        if (!gm.gameObject.GetComponent<PlayerResources>().isHealReady())
        {
            gm.gameObject.GetComponent<PlayerResources>().setHealTimer(0f); 

        }

        if (!gm.gameObject.GetComponent<PlayerResources>().isDashReady())
        {
            gm.gameObject.GetComponent<PlayerResources>().setDashTimer(0f);

        }
        if (!gm.gameObject.GetComponent<PlayerResources>().isKnockoutReady())
        {
            gm.gameObject.GetComponent<PlayerResources>().setKnockoutTimer(0f);  

        }
        if (!gm.gameObject.GetComponent<PlayerResources>().isChangeReady())
        {
           gm.gameObject.GetComponent <PlayerResources>().setChangeTimer(0f);    
        } 
        Destroy(buff);
    }

   

}