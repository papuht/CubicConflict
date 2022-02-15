using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class ShapeUIHandler : MonoBehaviour {

    private static float maxWidth = 5.2f; 

    private GameObject cooldown;
    private GameObject healthBar;

    void Start() {
        this.healthBar = this.gameObject.transform.Find("Health").gameObject;
        this.cooldown = this.gameObject.transform.Find("Cooldown").gameObject;
    }

    public void refresh(GameObject gm) {
        if(gm == null) {
            return;
        }

        //Set Cooldown indicator
        int cd = ((int) gm.GetComponent<PlayerResources>().getAbilityCooldown());
        cooldown.GetComponent<Text>().text = (cd <= 0 ? "R" : cd.ToString());
        cooldown.GetComponent<Text>().color = (cd <= 0 ? Color.green : Color.black);

        //Set hp bar state
        int maxHp = gm.GetComponent<PlayerResources>().getBaseHitpoints();
        int currentHp = gm.GetComponent<PlayerResources>().getHitpoints();
        float hpScale = ((float) currentHp) / ((float) maxHp);
        float widthCalc = maxWidth * hpScale;
        this.healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthCalc);

    }
}