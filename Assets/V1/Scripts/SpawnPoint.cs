using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    
    public GameObject player;
    public GameObject spawn;
    public double previousCheck;



    // Start is called before the first frame update
    void Start()
    {

        this.previousCheck = Time.time;

       
}

    // Update is called once per frame
    void Update()
    {

        float posX = spawn.transform.position.x;
        float posY = spawn.transform.position.y;
        Vector3 pos = new Vector2(posX, posY); 
         

        
        if (Time.time - this.previousCheck > 30) {
            Instantiate(this.player, pos, Quaternion.identity);
            player.tag = spawn.tag;
            this.previousCheck = Time.time;
            
        }

        
    }
}
