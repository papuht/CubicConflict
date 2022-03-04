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


    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Entering reload depot");

        if (collider.gameObject.tag != "Player")
        {
            return;
        }

        PlayerResources pr = collider.gameObject.GetComponent<PlayerResources>();

        //Check if river ms reduce effect is already in place, by trying to get the previous ms value
        //"" is the default value returned by the getter == empty
       if (!collider.gameObject.GetComponent<PlayerResources>().isHealReady())
        {
            collider.gameObject.GetComponent<PlayerResources>().setHealTimer(0f); 

        }

        if (!collider.gameObject.GetComponent<PlayerResources>().isDashReady())
        {
            collider.gameObject.GetComponent<PlayerResources>().setDashTimer(0f);

        }
        if (!collider.gameObject.GetComponent<PlayerResources>().isKnockoutReady())
        {
            collider.gameObject.GetComponent<PlayerResources>().setKnockoutTimer(0f);  

        }
        if (!collider.gameObject.GetComponent<PlayerResources>().isChangeReady())
        {
            collider.gameObject.GetComponent <PlayerResources>().setChangeTimer(0f);    

        }

    }

   

}