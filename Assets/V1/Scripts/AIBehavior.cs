using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
    private bool isIdle;
    private GameObject ai;


    GameObject captureMid; 
    GameObject captureBot; 
    GameObject captureTop;

    public ArrayList bottomdefense = new ArrayList();
    public ArrayList topdefense = new ArrayList();
    public ArrayList middefense = new ArrayList();

    private bool onTheTarget = false;

    // Start is called before the first frame update
    void Start() {
        //This is probaply the wanted outcome
        this.ai = this.gameObject;
        this.captureTop = GameObject.Find("CaptureTop");
        this.captureMid = GameObject.Find("CaptureMid");
        this.captureBot = GameObject.Find("CaptureBot");
        
    }

    // Update is called once per frame
    void Update() {
        ControlCheck();
        Debug.Log("Player 2 control value bottom:" + captureBot.GetComponent<Capture>().getPlayer2Control());
         
    }

    /*
     Checks if the AI -controlled pawn is idle. If idle, gives orders.  
     */
    

    /* change idle status to the parameter given*/
    private void toggleIdleStatus(bool isIdle) { 
        this.isIdle = isIdle;
    }

    private bool getIdleStatus() {
        return this.isIdle;
    }

    
    /* if enemy pawn enters the circular collider trigger, AI attacks*//*
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

    /* as long as enemy is in the circular collider trigger, AI attacks*//*
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
    }*/

    /* if enemy leaves the trigger area, AI sends its pawns to capture point*/

    /*
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
    */

    /* method for moving AI pawns*/
    private void Movement(Vector2 destination) {
        PlayerMovement.MovingObject move = new PlayerMovement.MovingObject(ai, destination);
        if(!move.move()) {
            //Clear force from a collision (or if the shape happens to be stuck)
            this.ai.GetComponent<PlayerResources>().resetMovement(false);
        }
    }

    private void ControlCheck() {

        if (this.ai.transform.position == captureBot.transform.position)
        {
            bottomdefense.Add(this.ai);
        }

        if (this.ai.transform.position == captureMid.transform.position) { 
        
            middefense.Add(this.ai);
        }

        if (this.ai.transform.position == captureTop.transform.position)
        {

            topdefense.Add(this.ai);
        }

        /*

        if (this.ai.transform.position != captureTop.transform.position)
        { 
            topdefense.Remove(this.ai);
        }

        if (this.ai.transform.position != captureMid.transform.position)
        {
            middefense.Remove(this.ai);
        }

        if (this.ai.transform.position != captureBot.transform.position)
        {
            bottomdefense.Remove(this.ai);
        }

        */


        if (bottomdefense.Count < 5 || captureBot.GetComponent<Capture>().getPlayer1Control() == true)
        {
            


                Vector3 destination = captureBot.transform.position;
                Movement(destination);



            
        }

                else if (middefense.Count <= 5  || captureMid.GetComponent<Capture>().getPlayer1Control() == true) {
            

                    Vector3 destination = captureMid.transform.position;
                    Movement(destination);
            
                }


                        else if (topdefense.Count < 5 || captureTop.GetComponent<Capture>().getPlayer1Control() == true)
                        {
            



                            Vector3 destination = captureTop.transform.position;
                             Movement(destination);

            


                         }

        















    }

}
