using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CollisionDamage : NetworkBehaviour {

    public Collider2D hitCollider;

    private void OnCollisionEnter2D(Collision2D collision) { 
        Collider2D collider1 = collision.collider;
        Collider2D collider2 = collision.otherCollider;

        if(!this.isClient || !this.hasAuthority) {
            return;
        }

        //Check if one of the colliders is already deleted
        if(collider1 == null || collider2 == null) {
            return;
        }
        
        if( //First we establish on the server that 2 enemy objects have collided
            collider1.gameObject.tag == collider2.gameObject.tag //This assumes that damage only affects shapes with thet tag Player
            && (collider1.gameObject.GetComponent<PlayerResources>().getPlayerId() != collider2.gameObject.GetComponent<PlayerResources>().getPlayerId())
        ) {
            Collider2D myObject, enemyObject;
                
            //Check which collider the Client has authority to
            if(collider1.gameObject.GetComponent<PlayerResources>().isOwner()) {
                myObject = collider1;
                enemyObject = collider2;
            }
            else {
                myObject = collider2;
                enemyObject = collider1;
            }

            //Handle singleplayer collisions that cant rely on authority
            if(
                collider1.gameObject.GetComponent<PlayerResources>().getPlayerId() == 666 
                || collider2.gameObject.GetComponent<PlayerResources>().getPlayerId() == 666 
            ) {
                if(collider1 != this.hitCollider) {
                    Debug.Log(collider1.gameObject.GetComponent<PlayerResources>().getPlayerId());
                    myObject = collider1;
                }
                else {
                    Debug.Log(collider2.gameObject.GetComponent<PlayerResources>().getPlayerId());
                    myObject = collider2;
                }
            }

            if (
                myObject == this.hitCollider 
                || (enemyObject == this.hitCollider )
            ) {
                //Handle collision and add a collision force
                CMDReduceHp(myObject.gameObject, 1);
                if (myObject.gameObject.GetComponent<PlayerResources>().isDead())
                {
                    SoundManagerScript.PlaySound ("Crack");
                    CMDDestroy(myObject.gameObject);
                }
                else {
                    SoundManagerScript.PlaySound ("Thud");
                    float avgMs = (
                    myObject.gameObject.GetComponent<PlayerResources>().getMovementSpeed()
                    + enemyObject.gameObject.GetComponent<PlayerResources>().getMovementSpeed()
                ) / 2;
                    //Force can be added localy since a NetworkRigidBodt2D component handles the force over the network
                    Vector2 direction = myObject.gameObject.transform.position - enemyObject.gameObject.transform.position;
                    myObject.gameObject.GetComponent<Rigidbody2D>().AddForce((direction * avgMs) * 5);
                    CMDResetMovement(myObject.gameObject);
                }
            }
        }
    }


    [Command] 
    private void CMDResetMovement(GameObject gm) {
        gm.GetComponent<PlayerResources>().resetMovement(true);
    }

    [Command] 
    private void CMDReduceHp(GameObject gm, int value) {
        //Debug.Log("Reduce HP called: " + gm.GetComponent<PlayerResources>().getPlayerId() + " | " + connectionToClient);
        gm.GetComponent<PlayerResources>().reduceHitpoints(value);
    }

    [Command] 
    private void CMDDestroy(GameObject gm) {
        //Debug.Log("Destroy called: " + gm.GetComponent<PlayerResources>().getPlayerId() + " | " + connectionToClient);
        Destroy(gm);
    }

}
