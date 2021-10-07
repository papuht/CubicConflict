using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionListener : MonoBehaviour {

    SelectionMap map;
    RaycastHit2D raycast;
    bool dragging = false;

    Vector2 start;
    Vector2 stop;

    void Start() {
        this.map = GetComponent<SelectionMap>();
    }

    void Update() {

        //On click
        if(Input.GetMouseButtonDown(0)) {
            this.start = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Debug.Log("Mouse click: " +Input.mousePosition.x + " " + Input.mousePosition.y);
        }

        //Held position
        /*
        if(
            Input.GetMouseButton(0) & (
            (this.start - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).magnitude > 40 ) 
        ) {
            this.dragging = true;
        }*/

        //On release
        if(Input.GetMouseButtonUp(0)) {
            this.raycast = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log("Raycast: " + this.raycast.collider);

        if(this.raycast.collider != null) {
            if(Input.GetKey(KeyCode.LeftShift)) {
                this.map.selectObject(this.raycast.transform.gameObject);
            }
            else {
                this.map.deselectAll();
                this.map.selectObject(this.raycast.transform.gameObject);
            }
        }
        else {
            this.map.deselectAll();
        }
           

            /*
            if(!this.dragging) {
                this.raycast = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                this.map.selectObject(this.raycast.transform.gameObject);
            }*/
        }
    }
}
