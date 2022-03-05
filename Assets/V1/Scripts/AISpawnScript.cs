using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AISpawnScript : NetworkBehaviour {
    private double lastCheck;

    public void resetTimer() {
        this.lastCheck = Time.time;
    }

    // Start is called before the first frame update
    void Start() {
        this.lastCheck = Time.time;
    }

    // Update is called once per frame
    void Update() {
        ConnectionResources cr = this.GetComponent<AIResources>();

        if(!cr.isReady()) {
            return;
        }

        if ((Time.time - this.lastCheck) > cr.getSpawnCooldown())
        {
            this.Spawn();
            this.lastCheck = Time.time;
        }
    }


    public void AssignGroup(GameObject ai) {
        int botCount = GetComponent<AIResources>().bottomdefense.Count;
        int midCount = GetComponent<AIResources>().middefense.Count;
        int topCount = GetComponent<AIResources>().topdefense.Count;
        int captureLimit = 2;

        //Do some randomizing in order to not be as predictable
        System.Random r = new System.Random();
        int c = r.Next(1, 4);
        if(c == 1 && botCount < captureLimit) {
            GetComponent<AIResources>().bottomdefense.Add(ai);
            Debug.Log("BottomDefense: (Randomizer) " + botCount);
            return;
        }
        else if(c == 2 && midCount < captureLimit) {
            GetComponent<AIResources>().middefense.Add(ai);
            Debug.Log("Middefense: (Randomizer) " + midCount);
            return;
        }
        else if(c == 3 && topCount < captureLimit) {
            GetComponent<AIResources>().topdefense.Add(ai);
            Debug.Log("Topdefense: (Randomizer)" + topCount);
            return;
        }

        //If randomizer didnt hit an empty spawn use ordered approach
        if (botCount < captureLimit){
            GetComponent<AIResources>().bottomdefense.Add(ai);
            Debug.Log("BottomDefense:" + botCount);
        }
        else if (midCount < captureLimit) {
            GetComponent<AIResources>().middefense.Add(ai);
            Debug.Log("Middefense:" + midCount);
        }
        else if (topCount < captureLimit) {
            GetComponent<AIResources>().topdefense.Add(ai);
            Debug.Log("Topdefense:" + topCount);
        }
        else {
            GetComponent<AIResources>().attackGroup.Add(ai);
            Debug.Log("Attackgroup:" + GetComponent<AIResources>().attackGroup.Count);
        }

    }



    [Command]
    public void Spawn() {

        //Note reference to AIResources
        ConnectionResources cr = GetComponent<AIResources>();
        

        GameObject spawnablePlayer = Instantiate(
            cr.getSpawnShape().prefab,
            cr.getSpawnPosition(),
            Quaternion.identity
        );

        PlayerResources pr = spawnablePlayer.GetComponent<PlayerResources>();

        //Set a unique id that we can compare on collision istead of tags
        pr.setPlayerId(cr.getPlayerId());

        //Set color to object
        pr.setColor(cr.getTeamColor());

        //Init default variables
        pr.initHitpoints(cr.getSpawnShape().hitpoints);
        pr.initMovementSpeed(cr.getSpawnShape().movementspeed, cr.getSpawnShape().maxMovementspeed);
        pr.initRotationSpeed(cr.getSpawnShape().rotationspeed, cr.getSpawnShape().maxRotationspeed);

        //Give player object a reference to ConnectionResources
        pr.setConnectionResources(cr);
        cr.addToPlayerObjects(spawnablePlayer);

        

        NetworkServer.Spawn(spawnablePlayer, connectionToClient);
        AssignGroup(spawnablePlayer);
    }


    
}
