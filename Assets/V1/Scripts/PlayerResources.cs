using UnityEngine;
using Mirror;

public class PlayerResources : NetworkBehaviour { 

    void Start() {
        this.setSpriteColor(this.color);
    }

    [SyncVar]
    private int id = -1;

    [SyncVar]
    private Color color = Color.white; 

    [SyncVar]
    private int hitpoints = 15;

    [SyncVar]
    private int movementspeed = 10;

    [SyncVar]
    private float rotationspeed = 390f;

    [SyncVar]
    public ConnectionResources cr;

    /**
    * All SyncVar settings have to happen on the server
    * Ie. You have to have Server rights when calling SET the methods
    * GET methods are always available since SyncVar auto updates the values when they are set on the server
    *
    * Server rights can be ensured by using a server only method or a command call
    */

    public int getPlayerId() {
        return this.id;
    }

    public ConnectionResources getConnectionResources() {
        return this.cr;
    }

    [Server]
    public void setConnectionResources(ConnectionResources cr) {
       this.cr = cr;
    }

    [Server]
    public void setPlayerId(int playerId) {
        this.id = playerId;
    }

    public bool isOwner() {
        return this.hasAuthority;
    }

    [Server]
    public void setColor(Color c) {
        this.color = c;
    }

    public Color getColor() {
        return this.color;
    }

    public void setSpriteColor(Color c) {
        this.GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    public Color getSpriteColor() {
        return this.GetComponentInChildren<SpriteRenderer>().material.color;
    }


    [Server]
    public void setHitpoints(int hitpoints) {
        this.hitpoints = hitpoints;
    }

    public int getHitpoints() {
        return this.hitpoints;
    }


    [Server]
    public void reduceHitpoints(int hitAmount) {
        this.hitpoints = hitpoints - hitAmount;
    }

    [Server]
    public void increaseHitpoints(int healAmount) {
        this.hitpoints = hitpoints + healAmount;
    }

    public bool isDead() {
        return (this.hitpoints <= 0);
    }

    [Server]
    public void setMovementSpeed(int ms) {
        this.movementspeed = ms;
    }

    public int getMovementSpeed() {
        return this.movementspeed;
    }

    [Server]
    public void setRotationSpeed(float rs) {
        this.rotationspeed = rs;
    }

    public float getRotationSpeed() {
        return this.rotationspeed;
    }
}