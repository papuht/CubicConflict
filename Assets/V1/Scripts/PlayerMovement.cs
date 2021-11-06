using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private SelectionMap map;
    Dictionary<int, MovingObject> movingObjects = new Dictionary<int, MovingObject>();
    
    void Start() {
        this.map = GetComponent<SelectionMap>(); //Map of selected objects
    }

    void Update() {

        //On mouse press save point and add all selected shapes into movemet group with said destination
        if(Input.GetMouseButtonUp(1)) {
            Vector2 click = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            
            foreach(KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {

                //Make sure GameObject still exists
                if(entry.Value != null) {

                    //If object is already in the movement group, give it a new destination
                    if(this.movingObjects.ContainsKey(entry.Key) && this.movingObjects[entry.Key] != null) {
                        Debug.Log("New destination for: " + entry.Value);
                        this.movingObjects[entry.Key].destination = click;
                    }
                    else { //Add selected object to movement group
                        Debug.Log("Adding to Movement group: " + entry.Value);

                        this.movingObjects.Add(
                            entry.Key, 
                            new MovingObject(
                                entry.Value, 
                                click, 
                                entry.Value.GetComponent<PlayerResources>().getMovementSpeed()
                            )
                        );
                        
                    }
                }
            }
        }

        //Stop movement of objects if 'S' is pressed
        if(Input.GetKeyUp(KeyCode.S)) {

            //On shift + s stop all movement
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
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
            if(entry.Value != null) {

                //Try to move and add to be removed if it fails
                if(!entry.Value.move()) { 
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
    private class MovingObject {
        public GameObject gameObject;
        public Vector2 destination;
        public Vector2 previousPosition;
        public int stuck = 0;
        public float previousDistance;
        public float previousCheck;

        //Store ms data for each object to future proof different ms for different shapes
        public float movementSpeed; 
        public MovingObject(GameObject gameObject, Vector2 destination, float movementSpeed) {
            this.gameObject = gameObject;
            this.destination = destination;
            this.movementSpeed = movementSpeed;
            this.previousCheck = Time.time;
        }

        //Move object towards destination depending on the movementSpeed
        public bool move() { 

            //Check if GameObject is Destroyed or the shape is stuck / has reached its destination
            if(this.gameObject == null || this.isStuck()) {
               return false;
            }

            //Move forward
            this.previousPosition = this.gameObject.transform.position;
            this.gameObject.transform.position = Vector2.MoveTowards(
                this.gameObject.transform.position,
                this.destination, 
                (this.movementSpeed * Time.deltaTime)
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
