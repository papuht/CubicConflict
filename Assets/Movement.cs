using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public float ms;

    public Rigidbody2D body;

    private Vector2 direction;

    SelectionMap map;

   void Start() {
        this.map = GetComponent<SelectionMap>();
   }
    void Update() {
        this.ProcessInput();
    }

    void FixedUpdate() {
        this.Move();
    }

    void Move() {
         foreach(KeyValuePair<int, GameObject> entry in this.map.getSelectedObjects()) {
            if(entry.Value != null) {
                Debug.Log("Moving: " + entry.Value);
                Rigidbody2D body = entry.Value.GetComponent<Rigidbody2D>();
                body.velocity = new Vector2(this.direction.x * this.ms, this.direction.y * this.ms);
            }
        }
    }

    void ProcessInput() {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        this.direction = new Vector2(x, y); 
    }
}
