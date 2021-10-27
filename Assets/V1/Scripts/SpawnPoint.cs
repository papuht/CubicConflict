using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//Note: classes that call network functions need NetworkBehaviour instead of MonoBehaviour
public class SpawnPoint : NetworkBehaviour { 

    /*
        In the newtork all the game instances have access to all the code
        However our network manager knows if the instance is a client
        Therefore we have to mark the methods with [Markers] that tell the manager,
        if the function should be ran on [Server], [Client] or called by client on server by [Command]
    */
    
    public GameObject player;
    private GameObject spawn;
    private double previousCheck;



    //This is called when player connects to host
    public override void OnStartLocalPlayer() { 
        /*
            We have to set the spawn object like this 
            since the script is already attached to a spawn object
        */
        //this.spawn = this.gameObject; 
    }

    void Start() {
        this.previousCheck = Time.time;
        this.spawn = this.gameObject;
    }

    void Update() {
        if(!hasAuthority) {
            return;
        }

        if (Time.time - this.previousCheck > 10) {
            this.SpawnOnServer();
            this.previousCheck = Time.time;
        }
    }

    [Command] //Command tag == This should be ran on the server, but the client commands it to do so
    public void SpawnOnServer() {
            float posX = spawn.transform.position.x;
            float posY = spawn.transform.position.y;
            Vector3 pos = new Vector2(posX, posY); 

            GameObject spawnablePlayer = Instantiate(this.player, pos, Quaternion.identity);
            //Set a unique id that we can compare on collision istead of tags
            spawnablePlayer.GetComponent<PlayerId>().set(connectionToClient.connectionId);
            //This spawns the object for all clients and also tells networkmanager who is the owner
            NetworkServer.Spawn(spawnablePlayer, connectionToClient); 
            
    }

}
