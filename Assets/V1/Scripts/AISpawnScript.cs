using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawnScript : MonoBehaviour
{
    GameObject spawningShape;
    private double lastCheck;

    public void resetTimer()
    {
        this.lastCheck = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.lastCheck = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        ConnectionResources cr = this.GetComponent<ConnectionResources>();

        if ((Time.time - this.lastCheck) > cr.getSpawnCooldown())
        {
            this.Spawn();
            this.lastCheck = Time.time;
        }
    }


    public void Spawn() {

        ConnectionResources cr = this.GetComponent<ConnectionResources>();
        GameObject spawnablePlayer = Instantiate(
               cr.getSpawnShape().prefab,
               GetComponent<ConnectionResources>().getSpawnPosition(),
               Quaternion.identity
           );

        PlayerResources pr = spawnablePlayer.GetComponent<PlayerResources>();

        //Set a unique id that we can compare on collision istead of tags
        pr.setPlayerId(666);

        //Set color to object
        pr.setColor(GetComponent<ConnectionResources>().p2TeamColors[Random.Range(0, this.GetComponent<ConnectionResources>().p2TeamColors.Length)]);

        //Init default variables
        pr.initHitpoints(cr.getSpawnShape().hitpoints);
        pr.initMovementSpeed(cr.getSpawnShape().movementspeed, cr.getSpawnShape().maxMovementspeed);
        pr.initRotationSpeed(cr.getSpawnShape().rotationspeed, cr.getSpawnShape().maxRotationspeed);

        //Give player object a reference to ConnectionResources
        pr.setConnectionResources(cr);

        cr.addToPlayerObjects(spawnablePlayer);


    }
}
