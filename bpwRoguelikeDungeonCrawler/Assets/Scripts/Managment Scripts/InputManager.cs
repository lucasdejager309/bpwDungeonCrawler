using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Axis {
    KeyCode negativeKey;
    KeyCode positiveKey;
    public Axis(KeyCode negativeKey, KeyCode positiveKey) {
        this.negativeKey = negativeKey;
        this.positiveKey = positiveKey;
    }

    public int GetAxis() {
        int intToReturn = 0;
        if (Input.GetKey(negativeKey)) {
            intToReturn -= 1;
        }
        if (Input.GetKey(positiveKey)) {
            intToReturn += 1;
        }
        return intToReturn;
    }

    public int GetAxisDown() {
        int intToReturn = 0;
        if (Input.GetKeyDown(negativeKey)) {
            intToReturn -= 1;
        }
        if (Input.GetKeyDown(positiveKey)) {
            intToReturn += 1;
        }
        return intToReturn;
    }
}

public class InputManager : Singleton<InputManager>
{
    public InputScriptableObject inputObject;

    void Awake() {
        Instance = this;
    }

    public Vector2Int GetInput(Axis xAxis, Axis yAxis, bool getKeyDown) {
        if (getKeyDown) {
            bool pressed = false;
            if (xAxis.GetAxisDown() != 0) pressed = true;
            if (yAxis.GetAxisDown() != 0) pressed = true;
    
            if (pressed) {
                return new Vector2Int(xAxis.GetAxis(), yAxis.GetAxis());
            } else return new Vector2Int(0, 0);
        } else {
            return new Vector2Int(xAxis.GetAxis(), yAxis.GetAxis());
        }
    }
}