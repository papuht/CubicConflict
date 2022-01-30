using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AISpawnScript : NetworkBehaviour
{
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
    }
}
