using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System;

//Note: classes that call network functions need NetworkBehaviour instead of MonoBehaviour
public class SpawnPoint : NetworkBehaviour { 

    /*
        In the newtork all the game instances have access to all the code
        However our network manager knows if the instance is a client
        Therefore we have to mark the methods with [Markers] that tell the manager,
        if the function should be ran on [Server], [Client] or called by client on server by [Command]
    */

    private GameObject triangleSpawnUI;
    private GameObject squareSpawnUI;
    private GameObject pentagonSpawnUI;

    private double lastCheck;

    private bool started = false;

    public void resetTimer() {
        this.lastCheck = Time.time;
    }

    void Start() {
        this.lastCheck = Time.time;
        this.triangleSpawnUI = GameObject.Find("SpawnImageTriangle");
        this.squareSpawnUI = GameObject.Find("SpawnImageSquare");
        this.pentagonSpawnUI = GameObject.Find("SpawnImagePentagon");

        this.squareSpawnUI.SetActive(false);
        this.pentagonSpawnUI.SetActive(false);

        //Only setup if not AI
        if(this.GetComponent<AIResources>() == null) {
            ControlRouter router = this.gameObject.GetComponent<ControlRouter>();
            router.connectCallback(ControlRouter.Key.S1, setSpawnShapeTriangle);
            router.connectCallback(ControlRouter.Key.S2, setSpawnShapeSquare);
            router.connectCallback(ControlRouter.Key.S3, setSpawnShapePentagon);
            router.connectCallback(ControlRouter.Key.S4, setSpawnShapeOctagon);
        }
        
    }

    void Update() {
        if(!hasAuthority) {
            return;
        }

        ConnectionResources cr = this.GetComponent<ConnectionResources>();

        if(!cr.isReady()) {
            return;
        }
        else if(!started) {
            GetComponent<AudioSource>().Play();
            this.started = true;
        }

        //A seperate UI update here since we need the accurate CD for it
        GameObject.Find("SpawnTimer").GetComponent<Text>().text = Convert.ToInt32((cr.getSpawnCooldown() - (Time.time - this.lastCheck))).ToString();
        
        if ((Time.time - this.lastCheck) > cr.getSpawnCooldown()) {
            this.SpawnOnServer();
            this.lastCheck = Time.time;
        }
    }
    public void setSpawnShapeTriangle() {
        ConnectionResources cr = this.GetComponent<ConnectionResources>();

        this.SwapSpawnOnServer("Triangle");
        Debug.Log(
            "Current shape: " + cr.getSpawnShape().prefab + 
            " | Current cd: " + cr.getSpawnCooldown()
        );
        this.triangleSpawnUI.SetActive(true);
        this.squareSpawnUI.SetActive(false);
        this.pentagonSpawnUI.SetActive(false);
    }
    public void setSpawnShapeSquare() {
        ConnectionResources cr = this.GetComponent<ConnectionResources>();

        this.SwapSpawnOnServer("Square");
        Debug.Log(
            "Current shape: " + cr.getSpawnShape().prefab + 
            " | Current cd: " + cr.getSpawnCooldown()
        );
        this.triangleSpawnUI.SetActive(false);
        this.squareSpawnUI.SetActive(true);
        this.pentagonSpawnUI.SetActive(false);
    }
    public void setSpawnShapePentagon() {
        ConnectionResources cr = this.GetComponent<ConnectionResources>();

        this.SwapSpawnOnServer("Pentagon");
        Debug.Log(
            "Current shape: " + cr.getSpawnShape().prefab + 
            " | Current cd: " + cr.getSpawnCooldown()
        );
        this.triangleSpawnUI.SetActive(false);
        this.squareSpawnUI.SetActive(false);
        this.pentagonSpawnUI.SetActive(true);
    }
    public void setSpawnShapeOctagon() {
        ConnectionResources cr = this.GetComponent<ConnectionResources>();

        this.SwapSpawnOnServer("Octagon");
        Debug.Log(
            "Current shape: " + cr.getSpawnShape().prefab + 
            " | Current cd: " + cr.getSpawnCooldown()
        );
        this.triangleSpawnUI.SetActive(false);
        this.squareSpawnUI.SetActive(false);
        this.pentagonSpawnUI.SetActive(true);
    }

    [Command]
    public void SwapSpawnOnServer(string shapeName) {
        bool result = GetComponent<ConnectionResources>().setSpawnShape(shapeName);
        Debug.Log("Swapping spawn to " + shapeName + " - " + result);
    } 

    [Command] //Command tag == This should be ran on the server, but the client commands it to do so
    public void SpawnOnServer() {

            ConnectionResources cr = GetComponent<ConnectionResources>();

            GameObject spawnablePlayer = Instantiate(
                cr.getSpawnShape().prefab, 
                GetComponent<ConnectionResources>().getSpawnPosition(), 
                Quaternion.identity
            );

            PlayerResources pr = spawnablePlayer.GetComponent<PlayerResources>();

            //Set a unique id that we can compare on collision istead of tags
            pr.setPlayerId(connectionToClient.connectionId);

            //Set color to object
            pr.setColor(GetComponent<ConnectionResources>().getTeamColor());

            //Save type of prefab to be referenced later
            pr.setType(cr.getSpawnShape().name);

            //Init default variables
            pr.initHitpoints(cr.getSpawnShape().hitpoints);
            pr.initMovementSpeed(cr.getSpawnShape().movementspeed, cr.getSpawnShape().maxMovementspeed);
            pr.initRotationSpeed(cr.getSpawnShape().rotationspeed, cr.getSpawnShape().maxRotationspeed);

            //Give player object a reference to ConnectionResources
            pr.setConnectionResources(cr);

            cr.addToPlayerObjects(spawnablePlayer);

            //This spawns the object for all clients and also tells networkmanager who is the owner
            NetworkServer.Spawn(spawnablePlayer, connectionToClient); 
    }


}
