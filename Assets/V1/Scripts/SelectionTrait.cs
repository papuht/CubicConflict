using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTrait : MonoBehaviour
{
    void Start() {
         GetComponent<Renderer>().material.color = Color.blue;
    }

    private void OnDestroy() {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void refresh() {
        if(GetComponent<Renderer>().material.color != Color.blue) {
            GetComponent<Renderer>().material.color = Color.blue;
        }
    }

}
