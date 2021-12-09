using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Mirror;

public class UI : NetworkBehaviour
{

    public GameObject hpText;
    public GameObject dashCooldown;
    public GameObject spawn;
    public GameObject spawnNext;
    public float lastCheck;
    SelectionMap selectionMap;

    // Start is called before the first frame update
    public void Start()
    {
        hpText = GameObject.Find("Hitpoints");
        dashCooldown = GameObject.Find("DashCd");
        spawnNext = GameObject.Find("NextSpawn");
        hpText.GetComponent<Text>().text = "Hitpoints: -";
        dashCooldown.GetComponent<Text>().color = Color.blue;
        dashCooldown.GetComponent<Text>().text = "Select Units..";
        GameObject.Find("SpawnTimer").GetComponent<Text>().text = "";
        selectionMap = GetComponent<SelectionMap>();
    }

    // Update is called once per frame
    void Update() {
        if(!this.isClient) {
            return;
        }

        if (selectionMap.getSelectedObjects().Count() >= 1)
        {
            hpText.GetComponent<Text>().enabled = true;
            dashCooldown.GetComponent<Text>().enabled = true;
            int temp = 0;
            dashCooldown.GetComponent<Text>().color = Color.red;
            dashCooldown.GetComponent<Text>().text = "Dash in cooldown";
            foreach (KeyValuePair<int, GameObject> entry in this.selectionMap.getSelectedObjects())
            {
                if (entry.Value != null)
                {
                    temp = temp + entry.Value.GetComponent<PlayerResources>().getHitpoints();
                    if (entry.Value.GetComponent<PlayerResources>().isDashReady())
                    {
                        dashCooldown.GetComponent<Text>().color = Color.green;
                        dashCooldown.GetComponent<Text>().text = "Dash is available.";
                    }
                }

            }
            hpText.GetComponent<Text>().text = "Hitpoints: " + temp.ToString();
        }
        else
        {
            hpText.GetComponent<Text>().text = "Hitpoints: -";
            dashCooldown.GetComponent<Text>().color = Color.blue;
            dashCooldown.GetComponent<Text>().text = "Select Units..";
        }

    }
}
