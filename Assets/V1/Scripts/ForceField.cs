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
    const string storageKey = "pre-river-ms";


    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Enter");

        if (collider.gameObject.tag != "Player")
        {
            return;
        }

        PlayerResources pr = collider.gameObject.GetComponent<PlayerResources>();

        //Check if river ms reduce effect is already in place, by trying to get the previous ms value
        //"" is the default value returned by the getter == empty
        if (pr.getFromStorage(storageKey) == "")
        {

            //Add old ms to storage with our key "pre-river-ms"
            pr.addToStorage(storageKey, pr.getMovementSpeed().ToString());

            //Old reduction logic
            pr.reduceMovementSpeed(20);
            if (pr.getMovementSpeed() < 3)
            {
                pr.setMovementSpeed(3);
            }
        }

    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player")
        {
            return;
        }

        //Now we get the saved value with the same key and set the ms according to it
        PlayerResources pr = collider.gameObject.GetComponent<PlayerResources>();
        //getAndDelete because keeping a huge list of random values for each shape is not good for the RAM :o
        string foundSpeed = pr.getAndDeleteFromStorage(storageKey);

        //Check that value wasn't deleted already (causes error while parsing if it is)
        if (foundSpeed != "")
        {
            //Since storage only allows for general string we have to parse the saved int put of it
            pr.setMovementSpeed(System.Int32.Parse(foundSpeed));
        }
    }

}