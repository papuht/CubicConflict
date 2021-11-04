using UnityEngine;
using Mirror;

public class PlayerId : NetworkBehaviour { 

    [SyncVar]
    private int id;

    [SyncVar]
    private Color teamColor; 
    public int get() {
        return this.id;
    }

    public void set(int playerId) {
        this.id = playerId;
    }

    [Client]
    public bool isOwner() {
        return this.hasAuthority;
    }


    public Color getTeamColor() {
        return this.teamColor;
    }

    public void setTeamColor(Color c) {
        this.teamColor = c;
    }

     void Start() {
         this.GetComponentInChildren<SpriteRenderer>().material.color = teamColor;
     }
}