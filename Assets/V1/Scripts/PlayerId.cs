using UnityEngine;
using Mirror;

public class PlayerId : NetworkBehaviour { 

    [SyncVar]
    private int id;
    public int get() {
        return this.id;
    }

    public void set(int playerId) {
        this.id = playerId;
    }
}