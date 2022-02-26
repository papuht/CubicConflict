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

   


    // Start is called before the first frame update
    void Start()
    {
        //This is probaply the wanted outcome
        this.ai = this.gameObject;
        this.captureTop = GameObject.Find("CaptureTop");
        this.captureMid = GameObject.Find("CaptureMid");
        this.captureBot = GameObject.Find("CaptureBot");

    }

    // Update is called once per frame
    void Update()
    {
        ControlCheck();
        

    }

    /*
     Checks if the AI -controlled pawn is idle. If idle, gives orders.  
     */


    /* change idle status to the parameter given*/
    private void toggleIdleStatus(bool isIdle)
    {
        this.isIdle = isIdle;
    }

    private bool getIdleStatus()
    {
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
    private void Movement(Vector2 destination, GameObject ai) {
        
        //Don't spam movement when destination has already been rechead
        if(System.Math.Abs(Vector2.Distance(ai.transform.position, destination)) < 3) {
            return;
        }

        //Reset collision forces
        ai.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ai.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        AIResources aiRes = (AIResources)GetComponent<PlayerResources>().getConnectionResources();
        PlayerMovement pm = aiRes.gameObject.GetComponent<PlayerMovement>();
        pm.remoteHandleMovingObject(ai, destination);
    }

    private void ControlCheck()
    {
        
        if (((AIResources)GetComponent<PlayerResources>().getConnectionResources()).bottomdefense.Contains(this.ai))
        {
            if (captureBot.GetComponent<Capture>().getPlayer1Control())
            {
                Vector3 destination = captureBot.transform.position;
                Movement(destination, this.ai);
            }
            else if (!captureBot.GetComponent<Capture>().getPlayer2Control())
            {
                Vector3 destination = captureBot.transform.position;
                Movement(destination, this.ai);
            }
            else if (captureBot.GetComponent<Capture>().getPlayer2Control())
            {
                Vector3 destination = captureBot.transform.position;
                Movement(destination, this.ai);
            }

        }

        if (((AIResources)GetComponent<PlayerResources>().getConnectionResources()).middefense.Contains(this.ai))
        {
            if (captureMid.GetComponent<Capture>().getPlayer1Control())
            {
                Vector3 destination = captureMid.transform.position;
                Movement(destination, this.ai);
            }
            else if (!captureMid.GetComponent<Capture>().getPlayer2Control())
            {
                Vector3 destination = captureMid.transform.position;
                Movement(destination, this.ai);
            }
            else if (captureMid.GetComponent<Capture>().getPlayer2Control())
            {
                Vector3 destination = captureMid.transform.position;
                Movement(destination, this.ai);
            }

        }

        if (((AIResources)GetComponent<PlayerResources>().getConnectionResources()).topdefense.Contains(this.ai))
        {
            if (captureTop.GetComponent<Capture>().getPlayer1Control())
            {
                Vector3 destination = captureTop.transform.position;
                Movement(destination, this.ai);
            }
            else if (!captureTop.GetComponent<Capture>().getPlayer2Control())
            {
                Vector3 destination = captureTop.transform.position;
                Movement(destination, this.ai);
            }
            else if (captureTop.GetComponent<Capture>().getPlayer2Control())
            {
                Vector3 destination = captureTop.transform.position;
                Movement(destination, this.ai);
            }

        }

        if (((AIResources)GetComponent<PlayerResources>().getConnectionResources()).attackGroup.Contains(this.ai))
        {
           if (captureMid.GetComponent<Capture>().getPlayer1Control())
                {
                    Vector3 destination = captureMid.transform.position;
                    Movement(destination, this.ai);
                }
           else if (captureMid.GetComponent<Capture>().getPlayer1Control())
            {
                Vector3 destination = captureMid.transform.position;
                Movement(destination, this.ai);
            }
           else if(captureBot.GetComponent<Capture>().getPlayer1Control())
                {
                Vector3 destination = captureBot.transform.position;
                Movement(destination, this.ai);
            }
        }
        
        

    }
}
