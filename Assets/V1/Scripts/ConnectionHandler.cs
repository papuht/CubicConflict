using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectionHandler : MonoBehaviour {

    //Main container
    public GameObject mainContainer;
    public Button mainBackBtn;
    public Text codeText;

    //Host container
    public GameObject hostContainer;
    public Button copyBtn;

    //Start container
    public GameObject startContainer;
    public Button hostBtn;
    public Button joinBtn;
    public Button hostBackBtn;

    //Client container
    public GameObject clientContainer;
    public GameObject clientPreConnectContainer;
    public InputField codeInput;
    public Button connectBtn;
    public Button clientBackBtn;
    public Text errorText;
    public Text clientStatusText;

    //Connection container
    public GameObject connectionContainer;
    public Text player1ConnectionText;
    public Text player2ConnectionText;
    private string connectToCode;
    private string connectWithCode;
    private string connectWithIP;

    void Start() {
        this.hostBtn.onClick.AddListener(StartHost);
        this.hostBackBtn.onClick.AddListener(stopHost);
        this.clientBackBtn.onClick.AddListener(StopClient);
        this.connectBtn.onClick.AddListener(StartClient);
        this.joinBtn.onClick.AddListener(
            () => { 
                this.setConnectionError("");
                this.clientStatusText.text = "";
                this.showClientContainer(true);
                this.showClientPreConnectContainer(true);
                this.showStartContainer(false);
            }
        );
        this.mainBackBtn.onClick.AddListener(
            () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); }
        );
        this.copyBtn.onClick.AddListener(
            () => { GUIUtility.systemCopyBuffer = this.connectToCode; }
        );
        
        this.showClientContainer(false);
        this.showHostContainer(false);
        this.showConnectionContainer(false);
    }

    public void setConnectionError(string msg) {
        this.errorText.color = Color.red;
        this.errorText.text = msg;
    }

    public void StartClient() {
        string rawInput = this.connectWithCode = this.codeInput.text;

        this.setConnectionError("");
        if(rawInput == "dev") {
            this.connectWithIP = "localhost";
        }
        else {
            if(rawInput == null || rawInput == "") {
                setConnectionError("Invalid code given");
                return;
            }

            try {
                this.connectWithIP = this.decodeIP(rawInput);
                if(rawInput == null || rawInput == "") {
                    setConnectionError("Invalid code given");
                    return;
                }   
            }
            catch {
                setConnectionError("Invalid code given");
                return;
            }
        }

        this.showClientPreConnectContainer(false);
        this.clientStatusText.text = "Connecting to: " + this.connectWithCode;
        this.showConnectionContainer(true);
        this.player2ConnectionText.color = Color.green;
        this.player2ConnectionText.text = "Player 2: Ready (Client)";

        NetworkManager.singleton.networkAddress = this.connectWithIP;
        NetworkManager.singleton.StartClient();
    }

    public void StopClient() {
        if (NetworkClient.isConnected) {
            NetworkManager.singleton.StopClient();
        }
        this.showClientContainer(false);
        this.showConnectionContainer(false);
        this.showStartContainer(true);
    }

    public void StartHost() {

        //Thread this action since it takes a bit to ping the website
        (new Thread(() => {
            string ip = this.getIP();
            this.connectToCode = this.encodeIP(ip);
        })).Start();

		NetworkManager.singleton.StartHost();
        this.showStartContainer(false);
        this.showHostContainer(true);
        this.showConnectionContainer(true);

        this.player1ConnectionText.color = Color.green;
        this.player1ConnectionText.text = "Player 1: Ready (Host)";
    }

    public void stopHost() {
        if (NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
        }
        this.showHostContainer(false);
        this.showConnectionContainer(false);
        this.showStartContainer(true);
    }

    public void showStartContainer(bool show) {
        startContainer.gameObject.SetActive(show);
    }

    public void showHostContainer(bool show) {
        hostContainer.gameObject.SetActive(show);
    }

    public void showClientContainer(bool show) {
        clientContainer.gameObject.SetActive(show);
    }

    public void showClientPreConnectContainer(bool show) {
        clientPreConnectContainer.SetActive(show);
    }

    public void showConnectionContainer(bool show) {
        connectionContainer.SetActive(show);
    }

    public void showMainContainer(bool show) {
        mainContainer.gameObject.SetActive(show);
    }

    public string getIP() {
        string htmlResponse = new WebClient().DownloadString("http://checkip.dyndns.org");
        if(htmlResponse == null) {
            return null;
        }
        string parsedIP = htmlResponse.Split(':')[1].Substring(1).Split('<')[0];
        return parsedIP;
    }

    public string encodeIP(string ip) {
        string encoded = "";
        foreach(string digit in ip.Split('.')) {
            string hexval = (System.Convert.ToInt32(digit)).ToString("x").ToUpper();
            encoded += (hexval.Length > 1) ? hexval :  hexval + "X";
        }
        return encoded;
    }

    public string decodeIP(string code) {
        string decoded = "";
        string part = "";
        foreach(char c in code.ToCharArray()) {
            part += c;
            if(part.Length >= 2) {  
                part = part.Replace("X", "");
                Debug.Log(part);
                int intval = System.Convert.ToInt32(part, 16);
                if(decoded != "") {
                    decoded += ".";
                }
                decoded += intval;
                part = "";
            }
        }
        return decoded;
    }

    void Update() {
        this.codeText.text = "Game code: " + this.connectToCode;
    }
}
