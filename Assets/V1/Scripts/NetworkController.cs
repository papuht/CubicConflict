using Mirror;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkManager {

    //TODO: Custom implementation for connection between clients and server

    public void OnClientConnect() {
        Debug.Log("Client connected");
    }
}
