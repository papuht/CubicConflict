using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
using UnityEngine.UI;

public class ConnectionResources : NetworkBehaviour {

    public Color[] p1TeamColors = new Color[] {Color.blue};
    public Color[] p2TeamColors = new Color[] {Color.red};

    public GameObject aiSpawner;

    protected int playerId;

    /**
    * Shape prehabs can be dragged here from unity editor
    * WARN! Remmeber to drag new prefabs also to NetworkManager's SpawnableObjects list
    * WARN! Objects added must have unique names 
    */
    public GameObject[] spawnablePrefabs;

    //Store all shapes that have been inited for spawning
    protected Dictionary<string, SpawnableShape> spawnableShapes = new Dictionary<string, SpawnableShape>();

    //Store all the shapes that a player currently has
    protected SyncList<GameObject> objectPool = new SyncList<GameObject>();

    [SyncVar]
    protected SpawnableShape spawnShape;

    [SyncVar]
    protected Color teamColor;

    [SyncVar]
    protected int spawnCooldown;

    [SyncVar]
    protected bool ready;

    [SyncVar]
    protected bool initCountdown;
    
    [SyncVar]
    protected double countdown;

    protected double countdownCheck;

    public SpawnableShape getSpawnShape() {
        return this.spawnShape;
    }

    [Server]
    public void setSpawnShape(SpawnableShape shape) {
        this.spawnShape = shape;
        this.spawnCooldown = this.spawnShape.cooldown; 
    }

    [Server]
    public bool setSpawnShape(string name) {
        SpawnableShape shape = this.getSpawnableShape(name);
        if(!shape.Equals(default(SpawnableShape))) {
            this.spawnShape = shape;
            this.spawnCooldown = this.spawnShape.cooldown; 
            return true;
        }
        return false;
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

    protected bool singleplayer = false;

    public virtual bool isReady() {
        return this.ready;
    }
    
    [Server]
    public void addToPlayerObjects(GameObject gm) {
        this.objectPool.Add(gm);
    }

    public SpawnableShape getSpawnableShape(string name) {
        try {
            return this.spawnableShapes[name];
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

    protected virtual void Start() {
        //Handle local singleplayer
        this.singleplayer = PlayerPrefs.GetInt("singleplayer") == 1 ? true : false; 

        if(this.singleplayer) {
            this.initCountdown = true;
            this.countdown = 5;
            this.countdownCheck = Time.time;

            this.spawnAI();
        }
    }
    
    protected virtual void Update() {
        if(!hasAuthority) {
            return;
        }
        if(!this.ready) {
            this.isGameReady();
            return;
        }
    }

    [Command]
    public void spawnAI() {
        //Spawn AISpawner
        GameObject aiPlayer = Instantiate(
            this.aiSpawner,
            new Vector2(
                -1 * this.gameObject.transform.position.x, 
                -1 * this.gameObject.transform.position.y
            ),
            Quaternion.identity
        );

        NetworkServer.Spawn(aiPlayer, connectionToClient); 

        //Init AI ConnectionResources
        aiPlayer.GetComponent<AIResources>().initAI(this);
    }

    [Command]
    protected void isGameReady() {
        if(!this.initCountdown && NetworkServer.connections.Count >= 2) {
            this.readyConnectionUI();
            this.initCountdown = true;
            this.countdown = 20;
            this.countdownCheck = Time.time;
        }
        if(this.initCountdown) {
            this.countdown -= (Time.time - this.countdownCheck);
            this.countdownCheck = Time.time;

            this.handleCountdownUI();
            
            if(this.countdown <= 0) {
                this.ready = true;
                this.initCountdown = false;
                
                this.handleCountdownEnd();
            } 
        }
    }

    [ClientRpc]
    protected void handleCountdownUI() {
        if(GameObject.Find("StartCountdown") != null) {
            Text hostText = GameObject.Find("StartCountdown").GetComponent<Text>();
            hostText.text = "> " + Convert.ToInt32(this.countdown).ToString() + " <";
        }
    }

     [ClientRpc]
    protected void handleCountdownEnd() {
        this.GetComponent<SpawnPoint>().resetTimer();
        if(GameObject.Find("ConnectionMainContainer") != null) {
            GameObject.Find("ConnectionMainContainer").SetActive(false);
        }
    }

    [ClientRpc]
    protected void readyConnectionUI() {

        if(GameObject.Find("HostConnectionStatus") != null) {
            Text hostText = GameObject.Find("HostConnectionStatus").GetComponent<Text>();
            hostText.color = Color.green;
            hostText.text = "Player 1: Ready (Host)";
        }

        if(GameObject.Find("ClientConnectionStatus") != null) {
            Text clientText = GameObject.Find("ClientConnectionStatus").GetComponent<Text>();
            clientText.color = Color.green;
            clientText.text = "Player 2: Ready (Client)";
        }

        if(GameObject.Find("ClientStatusText") != null) {
            Text clientStatus = GameObject.Find("ClientStatusHeader").GetComponent<Text>();
            clientStatus.text = clientStatus.text.Replace("Connecting to", "Connected to");
        }
        
    }

    public override void OnStartClient() {
         if (isServer) {
             Debug.Log("Player init started: " + connectionToClient.connectionId);
             this.playerId = connectionToClient.connectionId;

             if(connectionToClient.connectionId == 0) {
                this.teamColor = this.p1TeamColors[UnityEngine.Random.Range(0, this.p1TeamColors.Length)];
             }
             else {
                this.teamColor = this.p2TeamColors[UnityEngine.Random.Range(0, this.p2TeamColors.Length)];
             }

             this.spawnCooldown = 10;
             this.ready = false;
             this.initCountdown = false;

             this.initSpawnableObjects();
         }

         //Fetch Camera and set it to our spawn
        GameObject.Find("Main Camera").GetComponent<Camera>().transform.position = this.gameObject.transform.position;
     }


    protected void initSpawnableObjects() {
        Debug.Log("Starting Object list Init");
        foreach(GameObject prefab in this.spawnablePrefabs) {
           
           /**
           * Here you can add new handlers for new prefabs, take example from the case 'Player':
           * prefab.name === Prefab name in the file / unity editor Ie. 'Player'
           * New prefabs need to be added to spawnablePrefabs both in this script and NetworkManager
           * This can be done by dragging the prefab to the arrays in Unity editor
           */
           SpawnableShape shape;
           switch(prefab.name) {
                case "Triangle":
                    shape = new SpawnableShape {
                        prefab = prefab,
                        hitpoints = 10,
                        movementspeed = 10,
                        maxMovementspeed = 20,
                        rotationspeed = 250f,
                        maxRotationspeed = 350f,
                        cooldown = 5,
                        name = "Triangle"
                    };
                break;

                case "Square":
                    shape = new SpawnableShape {
                        prefab = prefab,
                        hitpoints = 10,
                        movementspeed = 10,
                        maxMovementspeed = 20,
                        rotationspeed = 250f,
                        maxRotationspeed = 350f,
                        cooldown = 5,
                        name = "Square"
                    };
                break;

                case "Pentagon":
                    shape = new SpawnableShape {
                        prefab = prefab,
                        hitpoints = 10,
                        movementspeed = 10,
                        maxMovementspeed = 20,
                        rotationspeed = 250f,
                        maxRotationspeed = 350f,
                        cooldown = 5,
                        name = "Pentagon"
                    };
                break;

                case "Octagon":
                    shape = new SpawnableShape{
                        prefab = prefab,
                        hitpoints = 10,
                        movementspeed = 10,
                        maxMovementspeed = 20,
                        rotationspeed = 250f,
                        maxRotationspeed = 350f,
                        cooldown = 5,
                        name = "Hexagon"
                    };
                break;

                default: //Default values we used during testing
                    shape = new SpawnableShape {
                        prefab = prefab, //Prefab comes from loop that we have defined to be 'Player' type
                        hitpoints = 10, //Set base hitpoints
                        movementspeed = 10, //Set base movementspeed
                        maxMovementspeed = 15, //Set max movementspeed
                        rotationspeed = 90f, //Set base rotationspeed
                        maxRotationspeed = 150f, //Set max rotationspeed
                        cooldown = 5, //Set spawn cooldown of this shape (in seconds)
                        name = "Default"
                    };
                break;
           }

           //Add to dictionary of spawnable objects
           this.spawnableShapes.Add(prefab.name, shape);
           Debug.Log(prefab.name + " Init: " + this.getSpawnableShape(prefab.name).prefab);
        }
        //Set spawnableObject Ie. what spawn script will spawn next
        this.setSpawnShape("Triangle");
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
        public int maxMovementspeed;
        public float rotationspeed;
        public float maxRotationspeed;
        public int cooldown;
        public string name;
        
    }

}
