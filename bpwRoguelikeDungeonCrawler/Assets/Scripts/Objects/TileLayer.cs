using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileLayer {

    public GenTile[] tiles;

    public Dictionary<Vector2Int, Tile> tileDictionary;

    public TileLayer() {
        tileDictionary = new Dictionary<Vector2Int, Tile>();
    }
}