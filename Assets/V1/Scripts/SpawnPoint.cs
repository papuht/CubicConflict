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

    private double lastCheck;

    private bool started = false;

    public void resetTimer() {
        this.lastCheck = Time.time;
    }

    void Start() {
        this.lastCheck = Time.time;
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

        //This is ugly but unity can't fetch the currently pressed keycode for a switch-case :(
        if(Input.GetKeyDown(KeyCode.A)) { 
            this.SwapSpawnOnServer("Triangle");
            Debug.Log(
                "Current shape: " + cr.getSpawnShape().prefab + 
                " | Current cd: " + cr.getSpawnCooldown()
            );
            GameObject.Find("NextSpawn").GetComponent<Text>().text = "Triangle";
        }
        else if(Input.GetKeyDown(KeyCode.S)) {
            this.SwapSpawnOnServer("Square");
            Debug.Log(
                "Current shape: " + cr.getSpawnShape().prefab + 
                " | Current cd: " + cr.getSpawnCooldown()
            );
            GameObject.Find("NextSpawn").GetComponent<Text>().text = "Square";
        }
        else if(Input.GetKeyDown(KeyCode.D)) {
            this.SwapSpawnOnServer("Pentagon");
            Debug.Log(
                "Current shape: " + cr.getSpawnShape().prefab + 
                " | Current cd: " + cr.getSpawnCooldown()
            );
            GameObject.Find("NextSpawn").GetComponent<Text>().text = "Pentagon";
        }
        else if(Input.GetKeyDown(KeyCode.F)) {
            this.SwapSpawnOnServer("Octagon");
            Debug.Log(
                "Current shape: " + cr.getSpawnShape().prefab + 
                " | Current cd: " + cr.getSpawnCooldown()
            );
            GameObject.Find("NextSpawn").GetComponent<Text>().text = "Octagon";

        }

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
