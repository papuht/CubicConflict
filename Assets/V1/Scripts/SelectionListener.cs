using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionListener : MonoBehaviour {

    private SelectionMap map;
    private SelectionDrawer drawer;
    private RaycastHit2D raycast;
    private bool dragging = false;

    private Vector2 start;
    private Vector2 stop;

    void Start() {
        this.map = GetComponent<SelectionMap>();
        this.drawer = new SelectionDrawer();
    }

    void Update() {

        //On click
        if(Input.GetMouseButtonDown(0)) {
            this.start = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Debug.Log("Mouse click: " +Input.mousePosition.x + " " + Input.mousePosition.y);
        }

        //Held position
        if(
            Input.GetMouseButton(0) & (
            (this.start - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).magnitude > 40 ) 
        ) {
            this.dragging = true;
            this.stop = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        //On release
        if(Input.GetMouseButtonUp(0)) {
            this.stop = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if(!this.dragging) { //Click select

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

            }
            else { //Mesh-select

                Vector3 wpStart = Camera.main.ScreenToWorldPoint(start);
                Vector3 wpStop = Camera.main.ScreenToWorldPoint(stop);

                //Center between 2 points
                Vector2 center = Vector2.Lerp(wpStart, wpStop, 0.5f );

                //Width and height of our box, ie. the difference in height and width between put 2 points
                float width = Mathf.Max(wpStart.x, wpStop.x) - Mathf.Min(wpStart.x, wpStop.x); 
                float height = Mathf.Max(wpStart.y, wpStop.y) - Mathf.Min(wpStart.y, wpStop.y);
                
                Vector2 size = new Vector2(width, height);
                if(!Input.GetKey(KeyCode.LeftShift)) {
                    this.map.deselectAll();
                }

                //Create a box in the set 'center' with the set 'size' and see what colliders get hit by said box
                RaycastHit2D[] casts = Physics2D.BoxCastAll(center,size, 0, Vector2.zero);
                foreach(RaycastHit2D hit in casts) {
                    if(hit.collider.GetType() == typeof(BoxCollider2D)) {
                        this.map.selectObject(hit.transform.gameObject);
                        Debug.Log("BoxCast: " + hit.collider);
                    }
                }
            }

            //Reset rectangle drawing stuff on release
            this.dragging = false;
        }
    }

    private void OnGUI() {
        if(this.dragging) {
            this.drawer.drawRectangle(this.start, this.stop, Color.white);
            this.drawer.DrawRectangleBorders(this.start, this.stop, 3, Color.grey);
        }
    }

}
