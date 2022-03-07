using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

/**
* A class that handles key maps and does callbacks when one of the mapped keys is pressed
*/
public class ControlRouter : MonoBehaviour {

    private Dictionary<Key, KeyValuePair<KeyCode, Action?>> shiftKeyMap;
    private Dictionary<Key, KeyValuePair<KeyCode, Action?>> ctrlKeyMap;
    private Dictionary<Key, KeyValuePair<KeyCode, Action?>> mainKeyMap;
    private KeyMap defaultKeyMap = new KeyMap {
        C1 = KeyCode.Alpha1, C2 = KeyCode.Alpha2, C3 = KeyCode.Alpha3, C4 = KeyCode.Alpha4,
        A1 = KeyCode.Q, A2 = KeyCode.W, A3 = KeyCode.E, A4 = KeyCode.R,
        S1 = KeyCode.A, S2 = KeyCode.S, S3 = KeyCode.D, S4 = KeyCode.F,
        M1 = KeyCode.Space, M2 = KeyCode.X, M3 = KeyCode.Z, M4 = KeyCode.C,
        U1 = KeyCode.Escape, U2 = KeyCode.Tab
    };

    void Start() {
        this.loadKeyMap(this.defaultKeyMap);
    }

    void Update() {
        
        //Choose which mapping to use based on modifier
        Dictionary<Key, KeyValuePair<KeyCode, Action?>> currentKeyMap = this.mainKeyMap;
        if(Input.GetKey(KeyCode.LeftShift)) {
            currentKeyMap = this.shiftKeyMap;
        }
        else if(Input.GetKey(KeyCode.LeftControl)) {
            currentKeyMap = this.ctrlKeyMap;
        }

        //Since modifiers are now established lets loop trough the correct keyMap
        foreach(KeyValuePair<Key, KeyValuePair<KeyCode, Action>> entry in currentKeyMap) {
            if(
                entry.Value.Value != null //Check if a callback funtion is registered
                && Input.GetKeyUp(entry.Value.Key) //Check if Registered key is currently pressed
            ) {
                entry.Value.Value(); //Use Callback function

                /**
                * Explanation:
                * entry == KeyValuePair<Key, KeyValuePair<KeyCode, Action>>
                * .Value == KeyValuePair<KeyCode, Action>
                * .Value() == Action() == Registered callback function
                */
            }
        } 
    }

    public void loadKeyMap(KeyMap map) {
        //Reset the key maps
        this.mainKeyMap = new Dictionary<Key, KeyValuePair<KeyCode, Action?>>();
        this.ctrlKeyMap = new Dictionary<Key, KeyValuePair<KeyCode, Action?>>();
        this.shiftKeyMap = new Dictionary<Key, KeyValuePair<KeyCode, Action?>>(); 

        FieldInfo[] fields = typeof(KeyMap).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach(FieldInfo field in fields) {
            mainKeyMap.Add(
                this.stringToKeyEnum(field.Name), 
                new KeyValuePair<KeyCode, Action?>((KeyCode)field.GetValue(map), null)
            );
            ctrlKeyMap.Add(
                this.stringToKeyEnum(field.Name), 
                new KeyValuePair<KeyCode, Action?>((KeyCode)field.GetValue(map), null)
            );
            shiftKeyMap.Add(
                this.stringToKeyEnum(field.Name), 
                new KeyValuePair<KeyCode, Action?>((KeyCode)field.GetValue(map), null)
            ); 
        }
    }   

    //mapIndex == KeyMap.attribute as a string, ie. c1 or m2
    public void connectCallback(Key keyEnum, Action methodToCall) {
        this.mainKeyMap[keyEnum] = new KeyValuePair<KeyCode, Action?>(
            this.mainKeyMap[keyEnum].Key, 
            methodToCall
        );
    }

    public void connectCallback(Modifier mod, Key keyEnum, Action methodToCall) {
        if(mod == Modifier.SHIFT) {
            this.shiftKeyMap[keyEnum] = new KeyValuePair<KeyCode, Action?>(
                this.shiftKeyMap[keyEnum].Key, 
                methodToCall
            );
        }
        else if(mod == Modifier.CTRL) {
            this.ctrlKeyMap[keyEnum] = new KeyValuePair<KeyCode, Action?>(
                this.ctrlKeyMap[keyEnum].Key, 
                methodToCall
            );
        }
    }

    public KeyCode getLoadedKey(Key keyEnum) {
        return this.mainKeyMap[keyEnum].Key;
    }

    public KeyCode getLoadedKey(Modifier mod, Key keyEnum) {
        if(mod == Modifier.SHIFT) {
            return this.shiftKeyMap[keyEnum].Key;
        }
        else {
            return this.ctrlKeyMap[keyEnum].Key;
        }
    }

    //Map for all available keys
    public struct KeyMap {
        public KeyCode C1; public KeyCode C2; public KeyCode C3; public KeyCode C4; //Control-groups 1-4
        
        public KeyCode A1; public KeyCode A2; public KeyCode A3; public KeyCode A4; //Abilities 1-4
        
        public KeyCode S1; public KeyCode S2; public KeyCode S3; public KeyCode S4; //Spawns 1-4

        //m1: Camera to base, m2: Rally point, m3: Spin up, m4: Spin down 
        public KeyCode M1; public KeyCode M2; public KeyCode M3; public KeyCode M4; //Misc. controls

        public KeyCode U1; public KeyCode U2; //UI controls
    }

    //Available modifiers
    public enum Modifier {
        SHIFT, CTRL
    }

    //Available keys
    public enum Key {
        C1, C2, C3, C4,
        A1, A2, A3, A4,
        S1, S2, S3, S4,
        M1, M2, M3, M4,
        U1, U2 
    }

    private Key stringToKeyEnum(string key) {
        Enum.TryParse(key, out Key parsedKey);
        return parsedKey;
    }
}