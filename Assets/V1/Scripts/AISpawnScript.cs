using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawnScript : MonoBehaviour
{
    public GameObject AIspawnObject;
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

       GameObject.Instantiate(
               AIspawnObject.GetComponent<ConnectionResources>().getSpawnShape().prefab,
               GetComponent<ConnectionResources>().getSpawnPosition(),
               Quaternion.identity
           );

        PlayerResources pr = AIspawnObject.GetComponent<PlayerResources>();

        //Set a unique id that we can compare on collision istead of tags
        pr.setPlayerId(666);

        //Set color to object
        pr.setColor(GetComponent<ConnectionResources>().p2TeamColors[Random.Range(0, this.GetComponent<ConnectionResources>().p2TeamColors.Length)]);

        //Init default variables
        pr.initHitpoints(AIspawnObject.GetComponent<ConnectionResources>().getSpawnShape().hitpoints);
        pr.initMovementSpeed(AIspawnObject.GetComponent<ConnectionResources>().getSpawnShape().movementspeed, AIspawnObject.GetComponent<ConnectionResources>().getSpawnShape().maxMovementspeed);
        pr.initRotationSpeed(AIspawnObject.GetComponent<ConnectionResources>().getSpawnShape().rotationspeed, AIspawnObject.GetComponent<ConnectionResources>().getSpawnShape().maxRotationspeed);

        //Give player object a reference to ConnectionResources
        

       


    }
}
