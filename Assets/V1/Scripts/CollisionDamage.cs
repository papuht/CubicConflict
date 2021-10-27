using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CollisionDamage : NetworkBehaviour {



    [Server] //[Server] == Run on server only, since we dont want clients to handle collision logic
    private void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider1 = collision.collider;
        Collider2D collider2 = collision.otherCollider;

        //Check if one of the colliders is already deleted
        if(collider1 == null || collider2 == null) {
            return;
        }

        if( //First we establish on the server that 2 enemy objects have collided
            collider1.gameObject.tag == collider2.gameObject.tag //This assumes that damage only affects shapes with thet tag Player
            && collider1.gameObject.GetComponent<PlayerId>().get() != collider2.gameObject.GetComponent<PlayerId>().get()
        ) {
            if ( //Case 1: Collider2 got hit with an edge
                collider1.GetType() == typeof(BoxCollider2D) 
                && collider2.GetType() == typeof(PolygonCollider2D)
            ) {
                Destroy(collider2.gameObject);
            }
            else if( //Case 2: Collider1 got hit with an edge
                collider1.GetType() == typeof(PolygonCollider2D) 
                && collider2.GetType() == typeof(BoxCollider2D)
            ) {
                Destroy(collider1.gameObject);
            } 
        }
      
    }

}
