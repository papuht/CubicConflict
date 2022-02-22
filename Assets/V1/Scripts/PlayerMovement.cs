using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour {
    private SelectionMap map;

    private List<KeyCombination> initedKeys = new List<KeyCombination>();

    private float dashTimer;
    private int dashCooldown = 8;
    Dictionary<int, MovingObject> movingObjects = new Dictionary<int, MovingObject>();
    
    [Command]
    public void CMDResetMovement(GameObject gm) {
        if(gm == null) {
            return;
        }
        gm.GetComponent<PlayerResources>().resetMovement(false);
    }

    [Command]
    public void CMDIncreaseMovement(GameObject gm, int value) {
        if(gm == null) {
            return;
        }
        gm.GetComponent<PlayerResources>().increaseMovementSpeed(value);
    }
    [Command]
    public void CMDDecreaseMovement(GameObject gm, int value) {
        if(gm == null) {
            return;
        }
        gm.GetComponent<PlayerResources>().reduceMovementSpeed(value);
    }
    [Command]
    public void CMDIncreaseRotation(GameObject gm, float value) {
        if(gm == null) {
            return;
        }
        gm.GetComponent<PlayerResources>().increaseRotationSpeed(value);
    }
    [Command]
    public void CMDDecreaseRotation(GameObject gm, float value) {
        if(gm == null) {
            return;
        }
        gm.GetComponent<PlayerResources>().reduceRotationSpeed(value);
    }
    [Command]
    public void CMDResetDash(GameObject gm) {
        if(gm == null) {
            return;
        }
        gm.GetComponent<PlayerResources>().resetDash();
    }

    [Command]
    public void CMDResetChange(GameObject gm)
    {
        if (gm == null)
        {
            return;
        }
        gm.GetComponent<PlayerResources>().resetChange();
    }

    [Command]
    public void CMDResetExpire(GameObject gm)
    {
        if (gm == null)
        {
            return;
        }
        gm.GetComponent<PlayerResources>().resetExpire();
    }

    void Start() {
        this.map = GetComponent<SelectionMap>(); //Map of selected objects
        this.dashTimer = Time.time;
        this.initUsableControls();
    }

    public override void OnStartClient() {
        this.dashTimer = Time.time;
    }   

    public void initUsableControls() {

        //Init simple key-inputs
        foreach(KeyCode k in (new KeyCode[] {
            //For optimization sake keep these in the most common usage order
            KeyCode.Q,
            KeyCode.W,
            KeyCode.E,
            KeyCode.R,
            KeyCode.Space,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.T,
            KeyCode.G,
            /*
            TODO: Need to move all controls to new class
            KeyCode.A,
            KeyCode.S,
            KeyCode.D,
            KeyCode.F,
            */
        })) {
            this.initedKeys.Add(new KeyCombination{
                key = k,
                modifier = null,
            });

            //Also add ctrl modifiers for below keys
            if(
                (new KeyCode[] {
                    KeyCode.Alpha1,
                    KeyCode.Alpha2,
                    KeyCode.Alpha3,
                    KeyCode.Alpha4,
                    KeyCode.Alpha5,
                }).Contains(k)
            ) {
                this.initedKeys.Add(new KeyCombination{
                    key = k,
                    modifier = KeyCode.LeftShift
                });
            }
        }
    
    }
    

    void Update() {
        if(!isLocalPlayer) {
            return;
        }
        if(Input.GetMouseButtonUp(1)) {
            this.handleMoveToClick();
        }
        this.handleKeyInput();
        this.handleAutoMove();
    }

    public void handleKeyInput() {
        //Loop inited keys see if one is pressed
        foreach(KeyCombination kc in this.initedKeys) {
            //Check if main key is pressed 
            if(Input.GetKeyUp(kc.key)) {

                //Input handler
                switch(kc.key) {
                    case KeyCode.Q:
                        this.handleDash();
                        break;
                    case KeyCode.W:
                        this.handleSizeChange(); //TODO: Add new abilities
                        break;
                    case KeyCode.E:
                        this.handleHealing(); //TODO: Add new abilities
                        break;
                    case KeyCode.R:
                        this.handleDash(); //TODO: Add new abilities
                        break;  
                    case KeyCode.Space:
                        //Space has another function elsewhere with modifier shift
                        if(!Input.GetKey(KeyCode.LeftShift)) {
                            this.handleShowSpawner();
                        }
                        break;
                    case KeyCode.Alpha1:
                        if(kc.modifier == null) {
                            this.handleControlGroupSelect(1);
                        }
                        else if(Input.GetKey((KeyCode) kc.modifier)) {
                            this.handleControlGroupUpdate(1);
                        }
                        break;
                    case KeyCode.Alpha2:
                        if(kc.modifier == null) {
                            this.handleControlGroupSelect(2);
                        }
                        else if(Input.GetKey((KeyCode) kc.modifier)) {
                            this.handleControlGroupUpdate(2);
                        }
                        break;
                    case KeyCode.Alpha3:
                        if(kc.modifier == null) {
                            this.handleControlGroupSelect(3);
                        }
                        else if(Input.GetKey((KeyCode) kc.modifier)) {
                            this.handleControlGroupUpdate(3);
                        }
                        break;
                    case KeyCode.Alpha4:
                        if(kc.modifier == null) {
                            this.handleControlGroupSelect(4);
                        }
                        else if(Input.GetKey((KeyCode) kc.modifier)) {
                            this.handleControlGroupUpdate(4);
                        }
                        break;
                    case KeyCode.Alpha5:
                        if(kc.modifier == null) {
                            this.handleControlGroupSelect(5);
                        }
                        else if(Input.GetKey((KeyCode) kc.modifier)) {
                            this.handleControlGroupUpdate(5);
                        }
                        break;
                    case KeyCode.T:
                        handleIncreaseRotation();
                        break;
                    case KeyCode.G:
                        handleDecreaseRotation();
                        break;
                }
            }
        }
    }

    public void handleControlGroupUpdate(int groupIndex) {
        Debug.Log("SET CONTROL GROUP " + groupIndex);
        this.map.setControlGroup(groupIndex);
    }

    public void handleControlGroupSelect(int groupIndex) {
        Debug.Log("USE CONTROL GROUP " + groupIndex);
        this.map.useControlGroup(groupIndex);
    }

    public void handleAutoMove() {
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

    public void handleMoveToClick() {
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

    public void handleStopMovement() {
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


    public void handleDash() {
        foreach (KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {
            //Make sure GameObject still exists
            if(entry.Value != null) {
                GameObject gm = entry.Value; 
                Vector2 mouse = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                float distance = gm.GetComponent<PlayerResources>().getMovementSpeed() / 2;

                //Try to dash
                if(gm.GetComponent<PlayerResources>().isDashReady() && gm.GetComponent<PlayerResources>().getType() == "Triangle") {
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

    public void handleIncreaseRotation() {
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

    public void handleDecreaseRotation() {
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

    public void handleShowSpawner() {
        GameObject.Find("Main Camera").GetComponent<Camera>().transform.position = this.gameObject.transform.position;
    }


    public void handleSizeChange() {

        foreach (KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects())
        {
           

            //Make sure GameObject still exists
            if (entry.Value != null)
            {
                GameObject gm = entry.Value;

                //Only works for pentagon shape and only if it's not already changed
                if(gm.GetComponent<PlayerResources>().getType() == "Pentagon" && (gm.GetComponent<PlayerResources>().getIsSizeChanged() == false))

                {
                    
                    Vector2 temp;
                                    
                    temp = gm.transform.localScale;
                    gm.GetComponent<PlayerResources>().setOriginalSize(temp);
                    temp.x += 3.0f;
                    temp.y += 3.0f;




                    if (gm.GetComponent<PlayerResources>().isChangeReady())
                    {
                        gm.transform.localScale = temp;
                        gm.GetComponent<PlayerResources>().setIsSizeChanged(true);
                        CMDResetChange(gm);
                        CMDResetExpire(gm);
                    }

                }
            }
        }

    }

    public void handleHealing()
    {

        foreach (KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects())
        {

            

            //Make sure GameObject still exists
            if (entry.Value != null)
            {
                GameObject gm = entry.Value;
                Debug.Log(gm.GetComponent<PlayerResources>().isHealReady());
                //Only works for square shape and only if the ability is ready
                if (gm.GetComponent<PlayerResources>().getType() == "Square" && gm.GetComponent<PlayerResources>().isHealReady())

                {
                   
                    RaycastHit2D[] res = new RaycastHit2D[100];
                    ContactFilter2D filter = new ContactFilter2D();
             
                    int healing = Physics2D.CircleCast(gm.gameObject.transform.position, 5.0f, gm.gameObject.transform.position, filter.NoFilter(), res);
                    Debug.Log("int healing:" +healing);
                    for (int i = 0; i < healing; i++)
                    {
                        if (res[i].collider.gameObject.GetComponent<PlayerResources>().getPlayerId() == gm.GetComponent<PlayerResources>().getPlayerId())
                        {
                            res[i].collider.gameObject.GetComponent<PlayerResources>().increaseHitpoints(30);
                            
                        }
                    }
                    gm.GetComponent<PlayerResources>().resetHeal();
                }
                }



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

    //Quick structure for storing a key + modifier combo
    //Eg. Shift + E
    public struct KeyCombination {
        public KeyCode key;
        public KeyCode? modifier; //Null if not used
    }

}
