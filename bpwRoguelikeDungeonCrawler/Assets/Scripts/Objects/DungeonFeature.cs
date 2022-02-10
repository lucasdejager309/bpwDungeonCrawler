using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class DungeonFeature : GenTile {
    public bool wallAdjacent;

    //copying and pasting this function from GenTiles is not very elegant
    public static DungeonFeature PickRandomFeature(DungeonFeature[] features) {
        //how the fuck does relative probability work??
        //https://forum.unity.com/threads/random-item-spawn-using-array-with-item-rarity-variable.176234/
        
        DungeonFeature pickedTile = null;
        float probabilitySum = 0;
        
        //get sum of probabilities
        foreach(DungeonFeature feature in features) {
            probabilitySum += feature.spawnChance;
        }

        //generate random number
        float randomFloat = Random.Range(0, probabilitySum+1);

        //pick tile
        for (int i = 0; i < features.Length && randomFloat > 0; i++) {
            randomFloat -= features[i].spawnChance;
            pickedTile = features[i];
        }
        if (pickedTile == null) {
            pickedTile = features[features.Length-1];
        }
     
        return pickedTile;
    }
}