using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //Leaving variables public so they can be controlled from unity
    public float cameraSpeed; 
    public float cameraMaxZoom = 25;
    public float cameraMinZoom = 10;
    public double moveOnTop;
    public double moveOnBottom;
    public double moveOnLeft;
    public double moveOnRight;
    private bool pause = false;

    private Vector2 minLimits;
    private Vector2 maxLimits;

    void Start() {
        this.cameraSpeed = 30;
        this.moveOnTop = Screen.height * 0.97; //Top 3% of the screen
        this.moveOnBottom = Screen.height * 0.03; //Bottom 3% of the screen
        this.moveOnRight = Screen.width * 0.97; //Right 3% of the screen
        this.moveOnLeft = Screen.width * 0.03; //Left 3% of the screen

        //Min bounds for camera
        this.minLimits = new Vector2(
            GameObject.Find("MinX").GetComponent<Collider2D>().bounds.max.x, //Walls of the map
            GameObject.Find("MinY").GetComponent<Collider2D>().bounds.max.y
        );

        //Max bounds for camera
        this.maxLimits = new Vector2(
            GameObject.Find("MaxX").GetComponent<Collider2D>().bounds.min.x,
            GameObject.Find("MaxY").GetComponent<Collider2D>().bounds.min.y
        );
    }   

    void Update() {
        //Pause camera with space since its annoying while developing
        if(Input.GetKeyUp(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift)) {
          this.toggleCameraPause();
        }

        if(!pause) {
            this.handleCameraScroll();
        }

        this.handleCameraZoom();

        this.correctCamera();
    }

    private void handleCameraZoom() {
        Camera camera = Camera.main;
        float zoom = camera.orthographicSize;

        if(
            (Input.mouseScrollDelta.y > 0) 
            && (zoom > this.cameraMinZoom)
        ) {
            Debug.Log("Camera zoomed out");
            camera.orthographicSize -= 1;
        }
        else if(
            (Input.mouseScrollDelta.y < 0) 
            && (zoom < this.cameraMaxZoom)
        ) {
            Debug.Log("Camera zoomed in");
            camera.orthographicSize += 1;
        }

    }

    private void handleCameraScroll() {
        Vector2 cameraPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        //Seconds since last frame * Camera speed variable
        float multiplier = Time.deltaTime * this.cameraSpeed;

        if(cameraPos.x >= this.moveOnRight) {
            //Move world space ie. camera in the world cordinates
            transform.Translate(Vector3.right * multiplier, Space.World);
        }
        else if(cameraPos.x <= this.moveOnLeft) {
            transform.Translate(Vector3.left * multiplier, Space.World);
        }

        if(cameraPos.y >= this.moveOnTop) {
            transform.Translate(Vector3.up * multiplier, Space.World); 
        }
        else if(cameraPos.y <= this.moveOnBottom) {
            transform.Translate(Vector3.down * multiplier, Space.World);
        }
    }

    private void correctCamera() {

        GameObject cam = this.gameObject;

        if(cam.transform.position.z != -20f) {
            Debug.Log("z error");
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -20f);
        }

        //Calculate camera the height and width based on screen size
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Screen.width / Screen.height;
        
        //Clamp camera to calculated limits
        cam.transform.position = new Vector3(
            Mathf.Clamp(cam.transform.position.x, this.minLimits.x + halfWidth, this.maxLimits.x - halfWidth),
            Mathf.Clamp(cam.transform.position.y, this.minLimits.y + halfHeight, this.maxLimits.y - halfHeight),
            cam.transform.position.z
        );
    }

    public bool toggleCameraPause() {
        this.pause = !pause;

        if(this.pause) {
            Debug.Log("Camera paused");
        }
        else {
             Debug.Log("Camera resumed");
        }
        return pause;
    }
}
