using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkController : NetworkManager {
    
    public GameObject capture; //Set in unity 
    public GameObject gameStateHandler;

    

    //TODO: Custom implementation for connection between clients and server

    //Currently Spawners poll server for 2 connections before they start spawning
    //This behaviour should be moved here
    public override void OnStartServer() {
        base.OnStartServer();
        Debug.Log("SERVER INIT");


        GameObject handler = Instantiate(
            this.gameStateHandler, 
            (new Vector2(10, 10)), 
            Quaternion.identity
        );
        NetworkServer.Spawn(handler); 
        
        //Spawn Capture Points with a reference to game state handler
        foreach(Vector2 location in (new Vector2[3] {(new Vector2(0,0)), (new Vector2(15,22)), (new Vector2(-15, -22))})) {
            GameObject cp = Instantiate(
                    this.capture, 
                    location, 
                    Quaternion.identity
            );

            cp.GetComponent<Capture>().referenceGameStateHandler(handler);
            NetworkServer.Spawn(cp); 
        }
    }

}