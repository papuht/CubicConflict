using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMap : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedUnits = new Dictionary<int, GameObject>();
    
    public void selectObject(GameObject obj) {
        int id = obj.GetInstanceID();

        if(!selectedUnits.ContainsKey(id)) {
            selectedUnits.Add(id, obj);  //Add the new id to GameObject pair
            obj.AddComponent<SelectionTrait>();
        }
    }

    public bool containsObject(int id) {
        return this.selectedUnits.ContainsKey(id);
    }

    public void deselectObject(int id) {
        Destroy(this.selectedUnits[id].GetComponent<SelectionTrait>());
        this.selectedUnits.Remove(id);
    }

    public Dictionary<int, GameObject> getSelectedObjects() {
        return this.selectedUnits;
    }

    public void deselectAll() {
        foreach(KeyValuePair<int, GameObject> entry in this.selectedUnits) {
            if(entry.Value != null) {
                Debug.Log("Clearing: " + entry.Value);
                Destroy(this.selectedUnits[entry.Key].GetComponent<SelectionTrait>());
            }
        }
        this.selectedUnits.Clear();
    }
}
