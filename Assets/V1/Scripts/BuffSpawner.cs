using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuffSpawner : NetworkBehaviour {

    public GameObject buffPrefab; 

    private float p1Timer;
    private float p2Timer;

    private GameObject lastP1Buff;
    private GameObject lastP2Buff;

    private bool p1IsCd;
    private bool p2IsCd;

    private static int CD = 10;

    private Vector2 p1Spawn = new Vector2(-25, 20);
    private Vector2 p2Spawn = new Vector2(25, -20);

    void Start() {
        if(!this.isServer) {
            return;
        }
        this.p1Timer = this.p2Timer = Time.time;
        this.lastP1Buff = this.lastP2Buff = null;
        this.p1IsCd = this.p2IsCd = true;
    }

    void Update() {
        if(!this.isServer) {
            return;
        }
        
        if(this.lastP1Buff == null && !this.p1IsCd) {
            this.p1Timer = Time.time;
            this.p1IsCd = true;
            Debug.Log("P1 BUFF PICKED");
        }
        else if(this.p1IsCd && ((int) (Time.time - this.p1Timer)) > CD) {
            spawnOnServer(true);
            this.p1IsCd = false;
            Debug.Log("P1 BUFF SPAWNED");
        }

        if(this.lastP2Buff == null && !this.p2IsCd) {
            this.p2Timer = Time.time;
            this.p2IsCd = true;
            Debug.Log("P2 BUFF PICKED");

        }
        else if(this.p2IsCd && ((int) (Time.time - this.p2Timer)) > CD) {
            spawnOnServer(false);
            this.p2IsCd = false;
            Debug.Log("P2 BUFF SPAWNED");
        }
    }

    [Server]
    public void spawnOnServer(bool isP1) {
        Vector2 position = isP1 ? this.p1Spawn : this.p2Spawn;
        Debug.Log("TRYING TO SPAWN");
        GameObject newBuff = Instantiate(
            this.buffPrefab, 
            position, 
            Quaternion.identity
        );
        NetworkServer.Spawn(newBuff); 
        Debug.Log(newBuff);
        if(isP1) {
            this.lastP1Buff = newBuff;
        }
        else {
            this.lastP2Buff = newBuff;
        }
    }
}
