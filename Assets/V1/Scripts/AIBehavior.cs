using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
    private bool isIdle;
    private GameObject ai;
    
    //Static positions of in game captures
    public Vector3 captureMidPos = new Vector2(0,0);
    public Vector3 captureTopPos = new Vector2(15,22);
    public Vector3 captureBotPos = new Vector2(-15,-22);

    // Start is called before the first frame update
    void Start() {
        //This is probaply the wanted outcome
        this.ai = this.gameObject;
    }

    // Update is called once per frame
    void Update() {
        checkIdleStatus();
    }

    /*
     Checks if the AI -controlled pawn is idle. If idle, gives orders.  
     */
    private void checkIdleStatus() {

        //TODO: Got it moving like this needs a proper dix 

        checkPosition();

        //Original code:
        /*if (this.isIdle == true) {
            toggleIdleStatus(false);
            checkPosition();
        }*/
    }

    /*
     Checks where the pawn is, and if it's not in capture point makes it go there.
    */
    private void checkPosition() {
        if (this.ai.transform.position == this.captureMidPos) {
            toggleIdleStatus(true);
        }
        else {
            Vector2 destination = this.captureMidPos;
            Movement(destination);
        }
    }

    /* change idle status to the parameter given*/
    private void toggleIdleStatus(bool isIdle) { 
        this.isIdle = isIdle;
    }


    /* if enemy pawn enters the circular collider trigger, AI attacks*/
    private void OnTriggerEnter2D(Collider2D collider) {

        //Check to ignore spawners and other unwanted colliders
        if(collider.gameObject.tag != "Player") {
            return;
        }

        if (this.isIdle == true) {
            if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != ai.gameObject.GetComponent<PlayerResources>().getPlayerId()) {
                toggleIdleStatus(false);
                Vector2 destination = collider.transform.position;
                Movement(destination);
            }
        }
    }

    /* as long as enemy is in the circular collider trigger, AI attacks*/
    private void OnTriggerStay2D(Collider2D collider) {

        //Check to ignore spawners and other unwanted colliders
        if(collider.gameObject.tag != "Player") {
            return;
        }
        
        if (this.isIdle == true) {
            if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != ai.gameObject.GetComponent<PlayerResources>().getPlayerId()) {
                toggleIdleStatus(false);
                Vector2 destination = collider.transform.position;
                Movement(destination);
            }
        }
    }

    /* if enemy leaves the trigger area, AI sends its pawns to capture point*/
    private void OnTriggerExit2D(Collider2D collider) {
        
        //Check to ignore spawners and other unwanted colliders
        if(collider.gameObject.tag != "Player") {
            return;
        }

        if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != ai.gameObject.GetComponent<PlayerResources>().getPlayerId()) {
            toggleIdleStatus(true);
            Vector2 destination = this.captureMidPos;
            Movement(destination);
        }
    }


    /* method for moving AI pawns*/
    private void Movement(Vector2 destination) {
        PlayerMovement.MovingObject move = new PlayerMovement.MovingObject(ai, destination);
        if(!move.move()) {
            //Clear force from a collision (or if the shape happens to be stuck)
            this.ai.GetComponent<PlayerResources>().resetMovement(false);
        }
    }



}
