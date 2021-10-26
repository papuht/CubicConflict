using Mirror;
using UnityEngine;


public class PlayerNetworking : NetworkBehaviour {

    /*
        In the newtork all the game instances have access to all the code
        However our network manager knows if the instance is a client
        Therefore we have to mark the methods with [Markers]

    */

    public GameObject spawnfab;

    private bool spawned = false;

    public override void OnStartLocalPlayer() {
        //this.startSpawn();
    }

    void Update() {

        if (!isLocalPlayer) { 
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            this.CmdSpawn();
        }
    }

    [Command] //This means to send a request to Server
    void CmdSpawn() {
        GameObject gameobject = Instantiate(this.spawnfab);
        NetworkServer.Spawn(gameobject, connectionToClient);
    }
}
