using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTrait : MonoBehaviour
{
    void Start() {
        GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
    }

    private void OnDestroy() {
        Color originalColor = GetComponent<PlayerResources>().getColor();
        GetComponent<PlayerResources>().setSpriteColor(originalColor);
    }

    public void refresh() {
        if(GetComponentInChildren<SpriteRenderer>().material.color != Color.white) {
            GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
        }
    }

}
