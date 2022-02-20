using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class SpawnableObject {
    public int spawnChance;
    public GameObject gameObject;
    public bool wallAdjacent;
}

class PickRandom {
    public static SpawnableObject PickRandomObject(SpawnableObject[] objects) {
        //how the fuck does relative probability work??
        //https://forum.unity.com/threads/random-item-spawn-using-array-with-item-rarity-variable.176234/
        
        SpawnableObject pickedObject = null;
        float probabilitySum = 0;
        
        //get sum of probabilities
        foreach(SpawnableObject currentObject in objects) {
            probabilitySum += currentObject.spawnChance;
        }

        //generate random number
        float randomFloat = Random.Range(0, probabilitySum+1);


        foreach(SpawnableObject currentObject in objects) {
            if (randomFloat > 0) {
                randomFloat -= currentObject.spawnChance;
                pickedObject = currentObject;
            } else break;
        }

        if (pickedObject == null) {
            pickedObject = objects[objects.Length-1];
        }
     
        return pickedObject;
    }
}