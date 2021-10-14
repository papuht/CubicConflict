using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       

        if (collision.collider.GetType() == typeof(PolygonCollider2D) && collision.otherCollider.GetType() == typeof(BoxCollider2D)) {
            Destroy(collision.otherCollider.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }


}
