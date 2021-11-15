using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ConnectionResources : NetworkBehaviour {

    private int playerId;

    /**
    * Shape prehabs can be dragged here from unity editor
    * WARN! Remmeber to drag new prefabs also to NetworkManager's SpawnableObjects list
    * WARN! Objects added must have unique names 
    */
    public GameObject[] spawnablePrefabs;

    //Store all shapes that have been inited for spawning
    private Dictionary<string, SpawnableShape> spawnableShapes = new Dictionary<string, SpawnableShape>();

    //Store all the shapes that a player currently has
    private SyncList<GameObject> objectPool = new SyncList<GameObject>();

    [SyncVar]
    private SpawnableShape spawnShape;

    [SyncVar]
    private Color teamColor;

    [SyncVar]
    private int spawnCooldown;

    [SyncVar]
    private bool ready;

    public SpawnableShape getSpawnShape() {
        return this.spawnShape;
    }

    [Server]
    public void setSpawnShape(SpawnableShape shape) {
        this.spawnShape = shape;
    }


    public Color getTeamColor() {
        return this.teamColor;
    }

    [Server]
    public void setTeamColor(Color c) {
        this.teamColor = c;
    }

    public int getSpawnCooldown() {
        return this.spawnCooldown;
    }

    [Server]
    public void setSpawnCooldown(int cd) {
        this.spawnCooldown = cd;
    }

    public bool isReady() {
        return this.ready;
    }
    
    [Server]
    public void addToPlayerObjects(GameObject gm) {
        this.objectPool.Add(gm);
    }

    public SpawnableShape getSpawnableShape(string name) {
        try {
            return this.spawnableShapes["name"];
        }
        catch {
            return new SpawnableShape{};
        }
    }

    public Dictionary<string, SpawnableShape> getAllSpawnableShapes() {
        return this.spawnableShapes;
    }

    public List<GameObject> getAllPlayerObjects() {
        List<GameObject> re = new List<GameObject>();
        foreach(GameObject gm in this.objectPool) {
            re.Add(gm);
        }
        return re;
    }

    public int getPlayerId() {
        return this.playerId;
    }


    public Vector3 getSpawnPosition() {
        return this.gameObject.transform.position;
    }

    void Start() {}
    
    void Update() {
        if(!hasAuthority) {
            return;
        }

        if(!this.ready) {
            this.isGameReady();
            return;
        }
    }

    [Command]
    private void isGameReady() {
        if(NetworkServer.connections.Count >= 2) {
            this.ready = true;
            this.GetComponent<SpawnPoint>().resetTimer();
        }
    }

    public override void OnStartClient() {
         if (isServer) {
             Debug.Log("Player init started: " + connectionToClient.connectionId);
             this.playerId = connectionToClient.connectionId;
             this.teamColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
             this.spawnCooldown = 10;
             this.ready = true;

             this.initSpawnableObjects();
         }
     }


    private void initSpawnableObjects() {
        foreach(GameObject prefab in this.spawnablePrefabs) {
           
           /**
           * Here you can add new handlers for new prefabs, take example from the case 'Player':
           * prefab.name === Prefab name in the file / unity editor Ie. 'Player' (currently only prefab)
           * New prefabs need to be added to spawnablePrefabs both in this script and NetworkManager
           * This can be done by dragging the prefab to the arrays in Unity editor
           */
           SpawnableShape shape;
           switch(prefab.name) {
               case "Player":
                    shape = new SpawnableShape{
                        prefab = prefab, //Prefab comes from loop that we have defined to be 'Player' type
                        hitpoints = 10, //Set base hitpoints
                        movementspeed = 10, //Set base movementspeed
                        rotationspeed = 360f //Set roationspeed
                    };
                    break;

               default: //Default values we used during testing
                    shape = new SpawnableShape{
                        prefab = prefab,
                        hitpoints = 10,
                        movementspeed = 10, 
                        rotationspeed = 90f, 
                    };
                break;
           }

           //Add to dictionary of spawnable objects
           this.spawnableShapes.Add(prefab.name, shape);
        }

        //Set spawnableObject Ie. waht spawn script will spawn next
        this.setSpawnShape(this.spawnableShapes["Player"]);
    }

    /**
    * A sub structure to store default values needed at spawn
    * Ie. Base MovementSpeed, Hitpoints etc. 
    * Struct type variables can be fetched with 'spawnableObject.hitpoints'
    */
    public struct SpawnableShape {
        public GameObject prefab;
        public int hitpoints;
        public int movementspeed;
        public float rotationspeed;
        
    }

}
