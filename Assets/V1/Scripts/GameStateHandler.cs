using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class GameStateHandler : NetworkBehaviour {

    [SyncVar] 
    private int player1Score = 0;

    [SyncVar]
    private int player2Score = 0;

    public Text player1Text;

    public Text player2Text;

    public Text scoreText;

    public GameObject scoreContainer;

    public Text gameoverText;

    public bool gg = false;

    public GameObject capture;

    public GameObject p1EndUIContainer; //Set in editor

    public GameObject p2EndUIContainer; //Set in editor

    void Update() {

        if(!this.isServer) {
            return;
        }

        if(!this.gg) {
            if(this.player1Score > 0) {
                RpcUpdateP1(this.player1Score.ToString());
            }
            
            if(this.player2Score > 0) {
                RpcUpdateP2(this.player2Score.ToString());
            }

            if(this.player1Score >= 100) {
                this.handleGameEnd("p1");
                this.gg = true;
            }
            else if(this.player2Score >= 100) {
                this.handleGameEnd("p2");
                this.gg = true;
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateP1(string s) {
        this.player1Text.text = s;
    }

    [ClientRpc]
    public void RpcUpdateP2(string s) {
        this.player2Text.text = s;
    }

    [ClientRpc]
    public void handleGameEnd(string winner) {
        this.scoreContainer.GetComponent<Image>().enabled = false;
        this.scoreText.enabled = false;
        this.player1Text.enabled = false;
        this.player2Text.enabled = false;
        if(winner == "p1") {
            this.p1EndUIContainer.SetActive(true);
        }
        else {
            this.p2EndUIContainer.SetActive(true);
        }
    }

    override public void OnStartClient() {
        base.OnStartClient();

        this.player1Score = 0;
        this.player2Score = 0;

        this.player1Text = GameObject.Find("Player1Score").GetComponent<Text>();
        this.player1Text.text = player1Score.ToString();

        this.player2Text = GameObject.Find("Player2Score").GetComponent<Text>();
        this.player2Text.text = player2Score.ToString();

        this.scoreText = GameObject.Find("ScoreText").GetComponent<Text>();

        this.scoreContainer = GameObject.Find("ScoreHolder");

        this.gameoverText = GameObject.Find("GameOverText").GetComponent<Text>();
        
        Debug.Log("GAME STATE HANDLER INIT");
    } 

    [Server]
    public void increasePlayer1Score() {
        this.player1Score++;
    }
    
    [Server]
    public void increasePlayer2Score() {
        this.player2Score++;
    }

}
