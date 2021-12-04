using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Capture : NetworkBehaviour {

    public GameObject stateHandler;

    [SyncVar]
    private int player1 = 0;
    [SyncVar]
    private int player2 = 0;

    [SyncVar]
    private bool player1Control = false;
    [SyncVar]
    private bool player2Control = false;


    private int player1ID = 0; //Host is always 0
    private int player2ID = 0;

    private float p1Check = 0;
    private float p2Check = 0;

    public override void OnStartServer() { 
        Debug.Log("CapturePoint INIT");
    }

    void Update() {
        Control();
    }


    /*Called when block enters the capture point*/
    private void OnTriggerEnter2D(Collider2D collider) {
        if (
            this.isServer 
            //Additional checks to avoid bugs
            && collider.GetType() == typeof(PolygonCollider2D)
            && collider.gameObject != null
            && collider.gameObject.tag == "Player"
        ) {
            Debug.Log("CaptureDetect triggered: " + collider);
            this.CaptureDetect(collider);
        }
    }

    /*Called when block exits the capture point*/
    private void OnTriggerExit2D(Collider2D collider) {
        if (
            this.isServer
            //Additional checks to avoid bugs
            && collider.GetType() == typeof(PolygonCollider2D)
            && collider.gameObject != null
            && collider.gameObject.tag == "Player"
        ) {
            Debug.Log("ExitCounter triggered: " + collider);
            this.ExitCounter(collider);
        }
    }


    /* CaptureDetect() keeps track of blocks entering the area, which is recorded in to variables player1 and player2*/
    private void CaptureDetect(Collider2D collider) {
        if ( //When a new ID is met save it as player2
            this.player2ID == 0
            && collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != 0 
        ) {
             this.player2ID = collider.gameObject.GetComponent<PlayerResources>().getPlayerId();
        }
        
        if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() == this.player1ID){
            player1++; 
            Debug.Log("P1 enters the zone! | P1: " + player1 + " | P2: " + player2 + " |");
        }
        else if(collider.gameObject.GetComponent<PlayerResources>().getPlayerId() == this.player2ID) {
            player2++;
            Debug.Log("P2 enters the zone! | P1: " + player1 + " | P2: " + player2 + " |");
        }
    }

    /*
     * ExitCounter() keeps track of blocks leaving the capture point area in order to determine which player has supremacy
     */
    private void ExitCounter(Collider2D collider) {
        if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() == this.player1ID) {
            if (player1 > 0) {
                player1--;
            }
        }
        else if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() == this.player2ID) {
            if (player2 > 0) {
                player2--;
            }
        }
    }

    /*
    * Control() checks which player controls the capture point, and gives points to that player. Invoked once per Update()
    */
    private void Control() {
        if (player1 > player2) {
            player1Control = true;
            player2Control = false;

        }
        else if (player2 > player1) {

            player2Control = true;
            player1Control = false;

        }
        else if (player1 == player2) {
            player2Control = false;
            player1Control = false;

        }

        if (player1Control == true 
            && Time.time - this.p1Check > 2
        ) {
            GameObject.Find("GameStateHandler").GetComponent<GameStateHandler>().increasePlayer1Score();
            this.p1Check = Time.time;
        }
        else if(
            player2Control == true 
            && Time.time - this.p2Check > 2
        ){
            this.stateHandler.GetComponent<GameStateHandler>().increasePlayer2Score();
            this.p2Check = Time.time;
        }
    }

    
    public void CmdIncreaseP1(GameObject gm) {
       gm.GetComponent<GameStateHandler>().increasePlayer1Score();
    }

    public void CmdIncreaseP2(GameObject gm) {
       gm.GetComponent<GameStateHandler>().increasePlayer2Score();
    }

    public void referenceGameStateHandler(GameObject reference) {
        this.stateHandler = reference;
    }

}



