using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HitPoints : NetworkBehaviour
{
    [SyncVar]
    private int hitpoints = 50;
    public int get()
    {
        return this.hitpoints;
    }

    public void set(int hit)
    {
        this.hitpoints = hitpoints-hit;
    }
}