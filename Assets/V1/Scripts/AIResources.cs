using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
using UnityEngine.UI;

/**
* AI handling is a unique problem
* It needs access to the Players CR for syncing and a private one for other tasks like spawning
* This script mimics that by extending and requiring two seperate CR instances
*
* Basically when we call ConnectionResources via this. we need to init them first but they are unique
* And if we call the syncResources it will return the Player ConnectionResources
* Most methods that need to be used need to be overriden or inited with a unique value
*/
public class AIResources : ConnectionResources {

    private GameObject aiPlayerPrefab;

    private SpawnableShape aiShape;

    public List<GameObject> bottomdefense = new List<GameObject>();
    public List<GameObject> topdefense = new List<GameObject>();
    public List<GameObject> middefense = new List<GameObject>();
    public List<GameObject> attackGroup = new List<GameObject>();



    //Reference for the player ConnectionResources
    private ConnectionResources syncResources;

    //Called by the players ConnectionResources onStart
    public void initAI(ConnectionResources playerConnectionResources) {
        this.syncResources = playerConnectionResources;

        this.initAiShapes();

        this.playerId = 666;
        this.teamColor = this.p2TeamColors[UnityEngine.Random.Range(0, this.p2TeamColors.Length)];
    }

    //An example of relying on the player ConnectionResources for syncing
    //Note: Overriding methods in C# requires the parent method to be declared 'virtual'
    public override bool isReady() {
        return syncResources.isReady();
    }

    //Override On start function so nothing silly happens
    protected override void Start() {}

    protected override void Update() {
        GetComponent<PlayerMovement>().handleAutoMove();
        this.colletControlTrash();
    }

    private void colletControlTrash() {
        this.removeNull(this.bottomdefense);
        this.removeNull(this.middefense);
        this.removeNull(this.topdefense);
        this.removeNull(this.attackGroup);
    }

    private void removeNull(List<GameObject> list) {
        List<GameObject> removeMe = new List<GameObject>();
        foreach(GameObject gm in list) {
            if(gm == null) {
                removeMe.Add(gm);
            }
        }
        foreach(GameObject trash in removeMe) {
            list.Remove(trash);
        }
    }

    public override void OnStartClient() {}

    //If the AI needs more shapes this needs to be redone
    public void initAiShapes() {

        this.aiShape = new SpawnableShape {
            //Shape list set in unity editor
            prefab = this.spawnablePrefabs[0],
            hitpoints = 15,
            movementspeed = 11,
            maxMovementspeed = 16,
            rotationspeed = 200f,
            maxRotationspeed = 300f,
            cooldown = 8,
            name = "Square"
        };

        this.setSpawnShape(this.aiShape);
    }


   


}