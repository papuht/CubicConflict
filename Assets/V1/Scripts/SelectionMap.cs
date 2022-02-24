using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMap : MonoBehaviour
{
    public Dictionary<int, GameObject> selectedUnits = new Dictionary<int, GameObject>();

    public Dictionary<int, GameObject> selectedUnitsUI = new Dictionary<int, GameObject>();

    public List<GameObject> lastFrame = new List<GameObject>();

    public GameObject parentContainer;

    public GameObject UITriangle;

    public GameObject UISquare;

    public GameObject UIPentagon;

    public GameObject UIOctagon;

    public Dictionary<int, List<GameObject>> controlGroups = new Dictionary<int, List<GameObject>>();

    public Dictionary<string, GameObject> typeToPrefabMap = new Dictionary<string, GameObject>();

    void Start() {
        parentContainer = GameObject.Find("UIBackground");
        this.typeToPrefabMap.Add("Triangle", this.UITriangle);
        this.typeToPrefabMap.Add("Square", this.UISquare);
        this.typeToPrefabMap.Add("Pentagon", this.UIPentagon);
        this.typeToPrefabMap.Add("Octagon", this.UIOctagon);
    }

    void Update() { 
        //Find and delete UI Elements ones the hosts are deselected
        List<int> deleteLaterUI = new List<int>();
        foreach(KeyValuePair<int, GameObject> entry in this.selectedUnitsUI) {
            if(
                !this.selectedUnits.ContainsKey(entry.Key)
                || this.selectedUnits[entry.Key] == null
                || this.selectedUnits[entry.Key].GetComponent<PlayerResources>().getHitpoints() <= 0
            ) {
                deleteLaterUI.Add(entry.Key);
            }
        }

        foreach(int key in deleteLaterUI) {
            GameObject uiShape = this.selectedUnitsUI[key];
            Destroy(uiShape);
            this.selectedUnitsUI.Remove(key);
        }

        int index = 0;
        foreach(KeyValuePair<int, GameObject> entry in this.selectedUnits) {
            if(entry.Value != null) {
                if(this.selectedUnits[entry.Key].GetComponent<SelectionTrait>() == null) {
                    entry.Value.AddComponent<SelectionTrait>();
                }
                entry.Value.GetComponent<SelectionTrait>().refresh();

                if(this.selectedUnitsUI.ContainsKey(entry.Key)) {
                    //Update already existing UI
                    GameObject existingUI = this.selectedUnitsUI[entry.Key];
                    existingUI.transform.localPosition = new Vector3(
                        this.UITriangle.transform.position.x + (index > 10 ? (index * 50 - 550) : (index * 50)),
                        this.UITriangle.transform.position.y + (index > 10 ? -50 : 0),
                        this.UITriangle.transform.position.z
                    );
                    existingUI.GetComponent<ShapeUIHandler>().refresh(entry.Value);

                }
                else if(index <= 22) {
                    //Create new UI displays for selected shapes
                    GameObject shapeUI = Instantiate(
                        this.typeToPrefabMap[entry.Value.GetComponent<PlayerResources>().getType()], 
                        new Vector3(
                            this.UITriangle.transform.position.x + (index > 10 ? (index * 50 - 550) : (index * 50)),
                            this.UITriangle.transform.position.y + (index > 10 ? -50 : 0),
                            this.UITriangle.transform.position.z
                        ), 
                        Quaternion.identity
                    );
                    shapeUI.transform.SetParent(parentContainer.transform, false);
                    this.selectedUnitsUI.Add(entry.Key, shapeUI);
                    this.selectedUnitsUI[entry.Key].GetComponent<ShapeUIHandler>().refresh(entry.Value);
                }
                index++;
            }
            
        }
    }

    public void setControlGroup(int groupIndex) {
        List<GameObject> ctrlGroup = new List<GameObject>();
        foreach(KeyValuePair<int, GameObject> entry in this.selectedUnits) {
            ctrlGroup.Add(entry.Value); 
        }
        this.controlGroups[groupIndex] = ctrlGroup;
    }

    public void useControlGroup(int groupIndex) {
        if(!this.controlGroups.ContainsKey(groupIndex)) {
            return;
        }

        this.deselectAll();

        foreach(GameObject gm in this.controlGroups[groupIndex]) {
            this.selectObject(gm);     
        }
    }
    
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

    public void resetSelectTo(Dictionary<int, GameObject> groupToSelect) {
        this.deselectAll();
        this.selectedUnits = groupToSelect;
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
