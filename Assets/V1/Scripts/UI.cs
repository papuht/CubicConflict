using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UI : MonoBehaviour
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
        hpText.GetComponent<Text>().enabled = false;
        dashCooldown.GetComponent<Text>().enabled = false;
        selectionMap = GetComponent<SelectionMap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionMap.getSelectedObjects().Count() >= 1)
        {
            hpText.GetComponent<Text>().enabled = true;
            dashCooldown.GetComponent<Text>().enabled = true;
            int temp = 0;
            dashCooldown.GetComponent<Text>().text = "Dash is not available";
            foreach (KeyValuePair<int, GameObject> entry in this.selectionMap.getSelectedObjects())
            {
                if (entry.Value != null)
                {
                    temp = temp + entry.Value.GetComponent<PlayerResources>().getHitpoints();
                    if (entry.Value.GetComponent<PlayerResources>().isDashReady())
                    {
                        dashCooldown.GetComponent<Text>().text = "Dash is available";
                    }
                }

            }
            hpText.GetComponent<Text>().text = "Hitpoints " + temp.ToString();
        }
        else
        {
            hpText.GetComponent<Text>().enabled = false;
            dashCooldown.GetComponent<Text>().enabled = false;
        }

        spawnNext.GetComponent<Text>().text = "Spawning: " + GetComponent<ConnectionResources>().getSpawnShape().name;

    }
}
