using UnityEngine;
using Mirror;

public class PlayerResources : NetworkBehaviour {

    [SyncVar]
    public GameObject gm;

    void Start() {
        this.setSpriteColor(this.color);
        this.gm = this.gameObject;
    }

    private void Update() {
        //Handle abilities with an expiration timer
        this.handleSizeReversion();
        this.handleKnockoutStatus();
    }

    private SyncDictionary<string, string> storage = new SyncDictionary<string, string>();

    [SyncVar]
    private int id = -1;

    [SyncVar]
    private Color color = Color.white; 

    [SyncVar]
    private int hitpoints = 1; //Just set some values in case, so netowrk dosent throw errors

    [SyncVar]
    private int baseHitpoints = 1; 

    [SyncVar]
    private int movementspeed = 1;

    [SyncVar]
    private int baseMovementspeed = 1;

    [SyncVar]
    private int maxMovementspeed = 50;

    [SyncVar]
    private float rotationspeed = 1f;

    [SyncVar]
    private float baseRotationspeed = 1f;

    [SyncVar]
    private float maxRotationspeed = 1000f;

    [SyncVar]
    private ConnectionResources cr;

    [SyncVar]
    private float dashTimer = 0f;

    [SyncVar]
    private float changeTimer = 0f;

    [SyncVar]
    private float expireTimer = 0f;
    
    [SyncVar]
    private bool clearMovement;

    [SyncVar]
    private string type;

    [SyncVar]
    private bool isAI = false;

    [SyncVar]
    private bool isSizeChanged = false;

    [SyncVar]
    private Vector2 originalSize;

    [SyncVar]
    private float healTimer = 0f;
    
    [SyncVar]
    private float knockoutTimer  = 0f;

    [SyncVar]
    private bool knockoutEnabled = false;


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


    //MAKE SURE TO DELETE VALUES SAVED TO STORAGE
    public string getAndDeleteFromStorage(string key) {
       string re = ""; 
       if(this.storage.TryGetValue(key, out re)) {
           this.storage.Remove(key);
           return re;
       }
       return "";
    }

    //MAKE SURE TO DELETE VALUES SAVED TO STORAGE
    public string getFromStorage(string key) {
       string re = ""; 
       if(this.storage.TryGetValue(key, out re)) {
           return re;
       }
       return "";

    }

    [Server] //STORAGE IS FOR TEMPORARY VARIABLES, DELETE ON USE!
    public void addToStorage(string key, string value) {
       this.storage.Add(key, value);
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

    /*[Server]
    public void setPlayerId(int playerId) {
        this.id = playerId;
    }

    [Server]
    public bool isAI() {
        return this.isAI;
    }*/

    public bool isOwner() {
        return this.hasAuthority;
    }

    public bool isMovementReset() {
        return this.clearMovement;
    }

    [Server]
    public void resetMovement(bool reset) {
        this.clearMovement = reset;
    }

    [Server]
    public void setColor(Color c) {
        this.color = c;
    }

    public Color getColor() {
        return this.color;
    }


    [Server]
    public void setType(string type) {
        this.type = type;
    }

    public string getType() {
        return this.type;
    }

    public void setSpriteColor(Color c) {
        this.GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    public Color getSpriteColor() {
        return this.GetComponentInChildren<SpriteRenderer>().material.color;
    }


    [Server] //Only use when spawning
    public void initHitpoints(int hitpoints) {
        this.hitpoints = hitpoints;
        this.baseHitpoints = hitpoints;
    }


    /**
    * Return the inited value of the variable
    * Can be used to restore variable to original value
    */
    public int getBaseHitpoints() {
        return this.baseHitpoints;
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
        if(this.hitpoints > this.baseHitpoints) {
            this.hitpoints = this.baseHitpoints;
        }
    }

    public bool isDead() {
        return (this.hitpoints <= 0);
    }

    [Server] //Only use when spawning
    public void initMovementSpeed(int ms, int maxMs) {
        this.movementspeed = ms;
        this.baseMovementspeed = ms;
        this.maxMovementspeed = maxMs;
    }

    /**
    * Return the inited value of the variable
    * Can be used to restore variable to original value
    */
    public int getBaseMovementSpeed() {
        return this.baseMovementspeed;
    }

    /**
    * Return the inited max value of the variable
    * Value is automatically capped in setters
    */
    public int getMaxMovementSpeed() {
        return this.baseMovementspeed;
    }

    [Server]
    public void setMovementSpeed(int ms) {
        this.movementspeed = ms;

        if(this.movementspeed < 0) {
            this.movementspeed = 0;
        }
        else if(this.movementspeed > this.maxMovementspeed) {
            this.movementspeed = this.maxMovementspeed;
        }
    }

    public int getMovementSpeed() {
        return this.movementspeed;
    }

    [Server]
    public void reduceMovementSpeed(int slowAmount) {
        this.movementspeed = movementspeed - slowAmount;

        if(this.movementspeed < 0) {
            this.movementspeed = 0;
        }
    }

    [Server]
    public void increaseMovementSpeed(int boostAmount) {
        this.movementspeed = movementspeed + boostAmount;

        if(this.movementspeed > this.maxMovementspeed) {
            this.movementspeed = this.maxMovementspeed;
        }
    }

     [Server] //Only use when spawning
    public void initRotationSpeed(float rs, float maxRs) {
        this.rotationspeed= rs;
        this.baseRotationspeed = rs;
        this.maxRotationspeed = maxRs;
    }

    /**
    * Return the inited value of the variable
    * Can be used to restore variable to original value
    */
    public float getBaseRotationSpeed() {
        return this.baseRotationspeed;
    }

    /**
    * Return the inited max value of the variable
    * Value is automatically capped in setters
    */
    public float getMaxRotationSpeed() {
        return this.baseMovementspeed;
    }

    [Server]
    public void setRotationSpeed(float rs) {
        this.rotationspeed = rs;

        if(this.rotationspeed < 0f) {
            this.rotationspeed = 0f;
        }
        else if(this.rotationspeed > this.maxRotationspeed) {
            this.rotationspeed = this.maxRotationspeed;
        }
    }

    public float getRotationSpeed() {
        return this.rotationspeed;
    }

    [Server]
    public void reduceRotationSpeed(float slowAmount) {
        this.rotationspeed = rotationspeed - slowAmount;

        if(this.rotationspeed < 0f) {
            this.rotationspeed = 0f;
        }
    }

    [Server]
    public void increaseRotationSpeed(float boostAmount) {
        this.rotationspeed = rotationspeed + boostAmount;

        if(this.rotationspeed > this.maxRotationspeed) {
            this.rotationspeed = this.maxRotationspeed;
        }
    }

    [Server]
    public void resetDash() {
        this.dashTimer = Time.time;
    }
    //methods similar to resetDash to track reset timers
    [Server]
    public void resetChange() { 
        this.changeTimer = Time.time;

    }

    [Server]
    public void resetExpire()
    {
        this.expireTimer = Time.time;

    }

    private static int dashCD = 8;
    private static int healCD = 50;
    private static int growCD = 40;
    private static int knockoutCD = 30;

    public float getAbilityCooldown() {

        //Silly fix that trusts certain shapes are using certain abilities
        int flatCD = 0;
        switch(this.type) {
            case "Triangle": //Dash cd
                flatCD = (int) (dashCD - (Time.time - this.dashTimer));
                break;
            case "Square": //Heal cd
                flatCD = (int) (healCD - (Time.time - this.healTimer));
                break;
            case "Pentagon": //Grow cd
                flatCD = (int) (growCD - (Time.time - this.changeTimer));
                break; 
             case "Octagon": //TODO:
                flatCD = (int) (knockoutCD - (Time.time - this.knockoutTimer));
                break;        
        }

        return (flatCD < 0 ? 0 : flatCD);
    }

    public bool isDashReady() {
        if((Time.time - this.dashTimer) > dashCD) {
            return true;
        }
        return false;
    }


   //method to track time for the reloading of change
    public bool isChangeReady() {
        if ((Time.time - this.changeTimer) > growCD)
        {
            return true;
        }
        return false;

    }

    //Method to track time for the duration of the change
    public bool isChangeExpired() {

        if ((Time.time - this.expireTimer) > 20)
        {
            return true;
        }
        return false;


    }


    //methods to set and get the boolean that is used in checking for the change to be in effect
    [Server]
    public void setIsSizeChanged(bool value) {
        this.isSizeChanged = value;
    
    }

    public bool getIsSizeChanged() {
        return this.isSizeChanged;
    }

    //Methods used to store the original size of the object and revert back to it when change is over. 
    public void setOriginalSize(Vector2 x) { this.originalSize = x; }

    public Vector2 getOriginalSize() { return this.originalSize; }


    //Reverts the changed object back to original size when it's time. Called in Update
    public void handleSizeReversion()
    {
        //Debug.Log("IsSizeChanged:" + getIsSizeChanged());
        //Debug.Log("Expired:" + isChangeExpired());

        if (this.isSizeChanged && isChangeExpired()) {
            this.gm.transform.localScale = getOriginalSize();
            setIsSizeChanged(false);
        }
    }

    public bool isHealReady() {
        if ((Time.time - this.healTimer) > healCD) {
            return true;
        }
        return false;
    }

    [Server]
    public void resetHeal() {
        this.healTimer = Time.time;
    }

    public bool isKnockoutReady() {
         if ((Time.time - this.knockoutTimer) > knockoutCD) {
            return true;
        }
        return false;
    }

    public bool isKnockoutEnabled() {
        return this.knockoutEnabled;
    }

    [Server]
    public void setKnockoutActivation(bool active) {
        this.knockoutEnabled = active;
        this.resetKnockout();
    }

    public bool isKnockoutExpired() {
        if ((Time.time - this.knockoutTimer) > 5) {
            return true;
        }
        return false;
    }

    public void resetKnockout() {
        this.knockoutTimer = Time.time;
    }

    public void handleKnockoutStatus() {
        if(!this.isKnockoutEnabled()) {
            return;
        }

        if(this.isKnockoutExpired()) {
            this.setKnockoutActivation(false);
        }
        else {
            this.gameObject.GetComponentInChildren<SpriteRenderer>().material.color = Color.yellow;
        }
    }


    public void setHealTimer(float time) { 
        this.healTimer = time;
    }

    public void setKnockoutTimer(float time)
    {
        this.knockoutTimer = time;
    }

    public void setDashTimer(float time)
    {
        this.dashTimer = time;
    }

    public void setChangeTimer(float time)
    {
        this.changeTimer = time;
    }



    /*
     * deprecated methods
     * TODO: Delete when sure
     * 
    public void setIsHealing(bool healing) {
    
        this.isHealing = healing;
        if (this.isHealing) {

            resetHeal();
        }
    }


    public bool getIsHealing() {

        return this.isHealing;
    }
    */



}