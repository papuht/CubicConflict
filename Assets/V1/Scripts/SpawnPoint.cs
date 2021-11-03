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
    private double startCheck;

    [SyncVar]
    private bool start;

    [SyncVar]
    private Color teamColor;

    void Start() {
        this.previousCheck = Time.time;
        this.startCheck = Time.time;
        this.spawn = this.gameObject;
    }

     public override void OnStartClient() {
         if (isServer) {
             this.start = false;
             this.teamColor = Random.ColorHSV(); //Choose color on client start
         }
     }


    void Update() {
        if(!hasAuthority) {
            return;
        }

        if(!start) {
            this.readyToStart();
            return;
        }

        if (Time.time - this.previousCheck > 10) {
            this.SpawnOnServer();
            this.previousCheck = Time.time;
        }
    }

    [Command]
    public void readyToStart() {
        if(NetworkServer.connections.Count >= 2) {
            this.start = true;
            this.previousCheck = Time.time;
        }
        else {
            this.start =  false;
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
            //Set color to object
            spawnablePlayer.GetComponent<PlayerId>().setTeamColor(this.teamColor);
            //This spawns the object for all clients and also tells networkmanager who is the owner
            NetworkServer.Spawn(spawnablePlayer, connectionToClient); 
    }

}
