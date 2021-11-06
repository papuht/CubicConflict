using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Capture : NetworkBehaviour {

    [SyncVar] //[SyncVar] == Automatically keep this variable synced between clients
    public int counter1 = 0;
    [SyncVar]
    public int counter2 = 0;

    private int player1ID = 0; //Host is always 0
    private int player2ID = 0;

    private float p1Check = 0;
    private float p2Check = 0;

    public Text player1Score;

    public Text player2Score;

    public override void OnStartClient() {
        if(isServer) {
            player1Score.text = counter1.ToString();
            player2Score.text = counter2.ToString();
        }
    }

    void Update() {
        this.player1Score.text = counter1.ToString();
        this.player2Score.text = counter2.ToString();
    }


    //The server method throws warnings when its called on clients
    //Now we can check if caller is the server before calling the server only method
    private void OnCollisionStay2D(Collision2D collision) { 
        if(this.isServer) {
            this.handleCounter(collision);
        }
    }


    [Server] //[Server] == Run on server only, since we dont want clients to handle collision logic
    private void handleCounter(Collision2D collision) {

        //First check that collision is by a player object
        if(collision.gameObject.tag == "Player") {

            /*
                This is a dirty fix but lets assume that the hostID is always 0 -> Player1
                Then when capture detects another playerID it is set as player2 
                This will work but maybe in the future is better to set the players when the game starts
                and not dynamically like this
            */

            //Normal case of colliding with host ie. player1
            if (
                collision.gameObject.GetComponent<PlayerResources>().getPlayerId() == this.player1ID
                && (Time.time - this.p1Check > 2)
            ) {
                counter1++;
                this.p1Check = Time.time;
            }
            else if ( //Normal case of colliding with player2
                collision.gameObject.GetComponent<PlayerResources>().getPlayerId() == this.player2ID
                && (Time.time - this.p2Check > 2)
                && player2ID != 0
            ) {
                counter2++;
                this.p2Check = Time.time;
            }
            else if( //When a new ID is met save it as player2
                this.player2ID == 0 
                && (Time.time - this.p2Check > 2)
                && collision.gameObject.GetComponent<PlayerResources>().getPlayerId() != 0
            ) { 
                this.player2ID = collision.gameObject.GetComponent<PlayerResources>().getPlayerId();
                counter2++;
                this.p2Check = Time.time;
            }
            

        }
        
    }

}
