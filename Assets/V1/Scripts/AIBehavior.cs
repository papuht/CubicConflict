using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
    private bool isIdle;
    public GameObject ai;
    public GameObject capture;
     





    // Start is called before the first frame update
    void Start()
    {

        

        
    }

    // Update is called once per frame
    void Update()
    {
        checkIdleStatus();
    }


    /*
     Checks if the AI -controlled pawn is idle. If idle, gives orders.  
     
     */

    private void checkIdleStatus()
    {
        if (this.isIdle == true) {

            toggleIdleStatus(false);
            checkPosition();

        }


    }

    /*
     Checks where the pawn is, and if it's not in capture point makes it go there.
     
     */

    private void checkPosition() {

        if (this.ai.transform.position == capture.transform.position)
        {
            toggleIdleStatus(true);

        }
        else {

            Vector2 destination = capture.transform.position;
            Movement(destination);

        }
        
    }



    


    /* change idle status to the parameter given*/

    private void toggleIdleStatus(bool isIdle) { 
    
        this.isIdle = isIdle;
    
    
    }

    /* if enemy pawn enters the circular collider trigger, AI attacks*/

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (this.isIdle == true)
        {
            if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != ai.gameObject.GetComponent<PlayerResources>().getPlayerId())
            {
                toggleIdleStatus(false);
                Vector2 destination = collider.transform.position;
                Movement(destination);

            }
        }

    }

    /* as long as enemy is in the circular collider trigger, AI attacks*/

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (this.isIdle == true)
        {
            if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != ai.gameObject.GetComponent<PlayerResources>().getPlayerId())
            {
                toggleIdleStatus(false);
                Vector2 destination = collider.transform.position;
                Movement(destination);
                
            }
        }

    }



    /* if enemy leaves the trigger area, AI sends its pawns to capture point*/

    private void OnTriggerExit2D(Collider2D collider)
    {
       
            if (collider.gameObject.GetComponent<PlayerResources>().getPlayerId() != ai.gameObject.GetComponent<PlayerResources>().getPlayerId())
            {
                toggleIdleStatus(true);
                Vector2 destination = capture.transform.position;
                Movement(destination);

        }
        
    }


    /* method for moving AI pawns*/
    private void Movement(Vector2 destination) {



        PlayerMovement.MovingObject move = new PlayerMovement.MovingObject(ai, destination);
        move.move();
    
    
    }


}
