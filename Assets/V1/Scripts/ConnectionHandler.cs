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
     public Text clientStatusHeader;

    //Connection container
    public GameObject connectionContainer;
    public Text player1ConnectionText;
    public Text player2ConnectionText;
    private string connectToCode;
    private string connectWithCode;
    private string connectWithIP;

    private bool singleplayer;


    //End game screens
    public GameObject endGameBlueContainer;
    public Button endGameReturnBlue;
    public GameObject endGameRedContainer;
    public Button endGameReturnRed;

    //Exit container
    public Button exitGameButton;

    void Start() {
        this.hostBtn.onClick.AddListener(StartHost);
        this.hostBackBtn.onClick.AddListener(stopHost);
        this.clientBackBtn.onClick.AddListener(StopClient);
        this.connectBtn.onClick.AddListener(StartClient);
        this.endGameReturnRed.onClick.AddListener(killBothConnections);
        this.endGameReturnBlue.onClick.AddListener(killBothConnections);
        this.exitGameButton.onClick.AddListener(killBothConnections);

        this.joinBtn.onClick.AddListener(
            () => { 
                this.setConnectionError("");
                this.clientStatusText.text = "";
                this.clientStatusHeader.text = "";
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
        this.shoeEndGameBlueContainer(false);
        this.shoeEndGameRedContainer(false);

        this.singleplayer = PlayerPrefs.GetInt("singleplayer") == 1 ? true : false; 
        if(singleplayer) {
            startSinglePlayer();
        }
    }

    public void killBothConnections() {
        this.stopHost();
        this.StopClient();
    }

    public void startSinglePlayer() {
        this.StartHost();

        this.connectToCode =  "-";

        this.player1ConnectionText.color = Color.green;
        this.player1ConnectionText.text = "Player 1: Ready (Host)";

        this.player2ConnectionText.color = Color.green;
        this.player2ConnectionText.text = "Player 2: Ready (AI)";
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
                this.connectWithIP = rawInput;
                NetworkManager.singleton.networkAddress = this.connectWithIP;
                NetworkManager.singleton.StartClient();

                this.showClientPreConnectContainer(false);
                this.clientStatusText.text = "" + this.connectWithCode;
                this.clientStatusHeader.text = "Connecting to:";
                this.showConnectionContainer(true);
                this.player2ConnectionText.color = Color.green;
                this.player2ConnectionText.text = "Player 2: Ready (Client)";
            }
            catch {
                setConnectionError("Invalid code given");
                return;
            }
        }
        
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
            if(!this.singleplayer) {
                //string ip = this.getIP();
                //this.connectToCode = this.encodeIP(ip);
                this.connectToCode = Steamworks.SteamClient.SteamId.ToString();
               
            }
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

    public void shoeEndGameBlueContainer(bool show) {
        this.endGameBlueContainer.SetActive(show);
    }

    public void shoeEndGameRedContainer(bool show) {
        this.endGameRedContainer.SetActive(show);
    }

    public void showConnectionContainer(bool show) {
        connectionContainer.SetActive(show);
    }

    public void showMainContainer(bool show) {
        mainContainer.gameObject.SetActive(show);
    }

    public string encodeSteamID(string id) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(id);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public string decodeSteamID(string encodedId) {
        var base64EncodedBytes = System.Convert.FromBase64String(encodedId);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
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
        this.codeText.text = this.connectToCode;
    }
}
