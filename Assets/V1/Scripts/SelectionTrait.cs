using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTrait : MonoBehaviour
{
    void Start() {
        GetComponentInChildren<SpriteRenderer>().material.color = Color.black;
    }

    private void OnDestroy() {
        GetComponentInChildren<Renderer>().material.color = GetComponent<PlayerId>().getTeamColor();
    }

    public void refresh() {
        
        if(GetComponentInChildren<SpriteRenderer>().material.color != Color.black) {
            GetComponentInChildren<SpriteRenderer>().material.color = Color.black;
        }
    }

}
