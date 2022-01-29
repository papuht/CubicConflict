using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour {
    private SelectionMap map;

    private float dashTimer;
    private int dashCooldown = 8;
    Dictionary<int, MovingObject> movingObjects = new Dictionary<int, MovingObject>();
    
    [Command]
    public void CMDResetMovement(GameObject gm) {
        gm.GetComponent<PlayerResources>().resetMovement(false);
    }

    [Command]
    public void CMDIncreaseMovement(GameObject gm, int value) {
        gm.GetComponent<PlayerResources>().increaseMovementSpeed(value);
    }
    [Command]
    public void CMDDecreaseMovement(GameObject gm, int value) {
        gm.GetComponent<PlayerResources>().reduceMovementSpeed(value);
    }
    [Command]
    public void CMDIncreaseRotation(GameObject gm, float value) {
        gm.GetComponent<PlayerResources>().increaseRotationSpeed(value);
    }
    [Command]
    public void CMDDecreaseRotation(GameObject gm, float value) {
        gm.GetComponent<PlayerResources>().reduceRotationSpeed(value);
    }
    [Command]
    public void CMDResetDash(GameObject gm) {
        gm.GetComponent<PlayerResources>().resetDash();
    }

    void Start() {
        this.map = GetComponent<SelectionMap>(); //Map of selected objects
        this.dashTimer = Time.time;
    }

    
    public override void OnStartClient() {
        this.dashTimer = Time.time;
    }   

    void Update() {

        if(!isLocalPlayer) {
            return;
        }

        //On space send camera to spawn
        if(Input.GetKeyUp(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift)) {
            GameObject.Find("Main Camera").GetComponent<Camera>().transform.position = this.gameObject.transform.position;
        }

        //Dash the selected objects
        if(Input.GetKeyUp(KeyCode.W)) {
            foreach (KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {
                //Make sure GameObject still exists
                if(entry.Value != null) {
                    GameObject gm = entry.Value; 
                    Vector2 mouse = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                    float distance = gm.GetComponent<PlayerResources>().getMovementSpeed() / 2;

                    //Try to dash
                    if(gm.GetComponent<PlayerResources>().isDashReady()) {
                        gm.transform.position = Vector2.MoveTowards( 
                            gm.transform.position, 
                            mouse, 
                            distance
                        );

                        CMDResetDash(gm);

                        //Change destination a little further than the mouse so that dashing feels more smooth
                        if(
                            //dashed
                            this.movingObjects.ContainsKey(entry.Key) 
                            && this.movingObjects[entry.Key] != null
                        ) {
                            this.movingObjects[entry.Key].destination = new Vector2(mouse.x * 1.2f, mouse.y + 1.2f);
                        }
                    }

                }
            }
        }

        //Increase rotation decrease movement
        if(Input.GetKeyUp(KeyCode.Q)) {
            foreach (KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {
                //Make sure GameObject still exists
                if(entry.Value != null) {
                    GameObject gm = entry.Value; 
                    if(gm.GetComponent<PlayerResources>().getBaseMovementSpeed() - 5 < (gm.GetComponent<PlayerResources>().getMovementSpeed() - 1)) {
                        this.CMDDecreaseMovement(gm, 1);
                    }
                    this.CMDIncreaseRotation(gm, 20f);
                    Debug.Log(
                        "Updated movement - rs: " + gm.GetComponent<PlayerResources>().getRotationSpeed() 
                        + " | ms: " + gm.GetComponent<PlayerResources>().getMovementSpeed()
                    );                
                }
            }
        }

        //Increase rotation decrease movement
        if (Input.GetKeyUp(KeyCode.E)) {
            foreach (KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {
                //Make sure GameObject still exists
                if(entry.Value != null) {
                    GameObject gm = entry.Value;
                    this.CMDIncreaseMovement(gm, 1);
                    if(gm.GetComponent<PlayerResources>().getBaseRotationSpeed() - 100 < (gm.GetComponent<PlayerResources>().getRotationSpeed() - 20f)) {
                        this.CMDDecreaseRotation(gm, 20f);
                    }
                    Debug.Log(
                        "Updated movement - rs: " + gm.GetComponent<PlayerResources>().getRotationSpeed() 
                        + " | ms: " + gm.GetComponent<PlayerResources>().getMovementSpeed()
                    );
                }
            }
        }

        //On mouse press save point and add all selected shapes into movemet group with said destination
        if(Input.GetMouseButtonUp(1)) {
            Vector2 click = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            
            foreach(KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {

                //Make sure GameObject still exists
                if(entry.Value != null && entry.Value.gameObject != null) {

                    //Clear forces when adding a new direction
                    entry.Value.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    entry.Value.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;

                    Debug.Log("New movement registered: " + click);
                    //If object is already in the movement group, give it a new destination
                    if(this.movingObjects.ContainsKey(entry.Key) && this.movingObjects[entry.Key] != null) {
                        //Debug.Log("New destination for: " + entry.Value);
                        this.movingObjects[entry.Key].destination = click;
                    }
                    else { //Add selected object to movement group
                        //Debug.Log("Adding to Movement group: " + entry.Value);

                        this.movingObjects.Add(
                            entry.Key, 
                            new MovingObject(
                                entry.Value, 
                                click
                            )
                        );
                        
                    }
                }
            }
        }

        //Stop movement of objects if 'S' is pressed
        if(Input.GetKeyUp(KeyCode.S)) {

            //On shift + s stop all movement
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                this.movingObjects.Clear();
            }
            else {//Stop movement of selected objects
                foreach(KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {
                    if(this.movingObjects.ContainsKey(entry.Key) && this.movingObjects[entry.Key] != null) {
                        this.movingObjects.Clear();
                    }
                }
            }
        }

        //Go through moving objects and move them towards the destination
        //If movemenet returns false ie. gameobject is destroyed add it to be removed
        List<int> remove = new List<int>();
        foreach(KeyValuePair<int, MovingObject> entry in this.movingObjects) {

            if(entry.Value != null && entry.Value.gameObject) {

                if(entry.Value.gameObject.GetComponent<PlayerResources>().isMovementReset()) {
                    this.CMDResetMovement(entry.Value.gameObject);
                    remove.Add(entry.Key);
                }

                //Try to move and add to be removed if it fails
                if(!entry.Value.move()) { 
                    this.CMDResetMovement(entry.Value.gameObject);
                    remove.Add(entry.Key);
                }

            }
        }

        //Remove destroyed and finished objects
        foreach(int id in remove) { 
            this.movingObjects.Remove(id);
        }
        
    }

    //A sub-class to store the info of moving objects, much easier to handle than a double dictionary
    public class MovingObject {
        public GameObject gameObject;
        public Vector2 destination;
        public Vector2 previousPosition;
        public int stuck = 0;
        public float previousDistance;
        public float previousCheck;

        //Store ms data for each object to future proof different ms for different shapes
        public float movementSpeed; 
        public MovingObject(GameObject gameObject, Vector2 destination) {
            this.gameObject = gameObject;
            this.destination = destination;
            this.previousCheck = Time.time;
        }

        //Move object towards destination depending on the movementSpeed
        public bool move() { 

            //Check if GameObject is Destroyed or the shape is stuck / has reached its destination
            if(this.gameObject == null || this.isStuck() || this.gameObject.GetComponent<PlayerResources>().isMovementReset()) {
               return false;
            }

            //Move forward
            this.previousPosition = this.gameObject.transform.position;
            this.gameObject.transform.position = Vector2.MoveTowards(
                this.gameObject.transform.position,
                this.destination, 
                (this.gameObject.GetComponent<PlayerResources>().getMovementSpeed() * Time.deltaTime)
            );

            return true;
        }

        //Figure out when to stop spamming move command onto the shape
        //Ie. when the shape is stuck or it has reach the destination
        public bool isStuck() {
            //Every 0.05 seconds check the distance travelled
            if((Time.time - this.previousCheck) > 0.05) {

                //If distance has change less than 0.1 world distance in this time, the shape is stuck
                if(((System.Math.Abs(this.previousDistance - Vector2.Distance(this.gameObject.transform.position, this.destination))) < 0.1)) {
                    return true;
                }

                //Else reset the clock and the current distance to destination
                this.previousDistance = Vector2.Distance(this.gameObject.transform.position, this.destination);
                this.previousCheck = Time.time;
                return false;
            }
            else { 
                //If not enough time has passed since last check naturally let the shape move
                return false;
            }
            
        }

    }
}
