using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[System.Serializable]
public class GenTile {
    public Tile[] tiles;
    public int spawnChance;

    public Tile GetTile() {
        if (tiles.Length == 1) {
            return tiles[0];
        } else {
            return tiles[Random.Range(0, tiles.Length)];
        }
    }

    public static GenTile PickRandomGenTile(GenTile[] genTiles) {
        //how the fuck does relative probability work??
        //https://forum.unity.com/threads/random-item-spawn-using-array-with-item-rarity-variable.176234/
        
        GenTile pickedTile = null;
        float probabilitySum = 0;
        
        //get sum of probabilities
        foreach(GenTile genTile in genTiles) {
            probabilitySum += genTile.spawnChance;
        }

        //generate random number
        float randomFloat = Random.Range(0, probabilitySum+1);

        //pick tile
        for (int i = 0; i < genTiles.Length && randomFloat > 0; i++) {
            randomFloat -= genTiles[i].spawnChance;
            pickedTile = genTiles[i];
        }
        if (pickedTile == null) {
            pickedTile = genTiles[genTiles.Length-1];
        }
     
        return pickedTile;
    }
}