using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : MonoBehaviour
{


    public int counter1 = 0;
    public int counter2 = 0;


    // Start is called before the first frame update
    void Start()
    {

       
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log("Pelaaja 1:" + counter1);
        Debug.Log("Pelaaja 2:" + counter2);


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            counter1++;
        }
        if (collision.gameObject.tag == "Player2") {

            counter2++;
        }
    }

}
