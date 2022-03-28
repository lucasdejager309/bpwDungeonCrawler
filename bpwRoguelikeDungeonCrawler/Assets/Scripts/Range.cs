using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Range {
    Range(float min, float max) {
        this.min = min;
        this.max = max;
    }
    
    public float min;
    public float max;

    public float GetRandom() {
        return Random.Range(min, max);
    }
}