using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Rotation : NetworkBehaviour {




    void Update()
    {

       

        float rotAmount = this.GetComponent<PlayerResources>().getRotationSpeed() * Time.deltaTime;
        float curRot = transform.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curRot + rotAmount));


        
    }
}

