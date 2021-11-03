using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HitPoints : NetworkBehaviour
{
    [SyncVar]
    private int hitpoints = 15;
    public int get()
    {
        return this.hitpoints;
    }

    public void reduce(int hit)
    {
        this.hitpoints = hitpoints-hit;
        //Debug.Log(this.gameObject + " Got hit, HP: " + this.hitpoints);
    }
}