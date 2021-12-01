using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SlowGround : NetworkBehaviour
{

    int originalSpeed = 0;



    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnTriggerEnter2D(Collider2D collider)

    {

        if (this.originalSpeed == 0)
        {
            this.originalSpeed = collider.gameObject.GetComponent<PlayerResources>().getMovementSpeed();
        }


        collider.gameObject.GetComponent<PlayerResources>().reduceMovementSpeed(20);
        if (collider.gameObject.GetComponent<PlayerResources>().getMovementSpeed() < 3)
        {
            collider.gameObject.GetComponent<PlayerResources>().setMovementSpeed(3);
        }



    }

    public void OnTriggerStay2D(Collider2D collider)
    {
       
        collider.gameObject.GetComponent<PlayerResources>().reduceMovementSpeed(20);
        if (collider.gameObject.GetComponent<PlayerResources>().getMovementSpeed() < 3) {
            collider.gameObject.GetComponent<PlayerResources>().setMovementSpeed(3);
        }



    }



    public void OnTriggerExit2D(Collider2D collider)
    {

        collider.gameObject.GetComponent<PlayerResources>().setMovementSpeed(originalSpeed);




    }

    



}
