using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Minimap : MonoBehaviour
{
    public Tilemap minimapTileMap;
    public Tile floorTile;
    public Tile solidTile;

    public void SetDungeon() {
        minimapTileMap.ClearAllTiles();
        
        Dictionary<Vector2Int, Tile> floorTiles = DungeonGen.Instance.floorTilelayer.tileDictionary;
        Dictionary<Vector2Int, Tile> solidTiles = DungeonGen.Instance.solidTileLayer.tileDictionary;

        foreach(KeyValuePair<Vector2Int, Tile> entry in floorTiles) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                minimapTileMap.SetTile(location, floorTile);
        }

        foreach(KeyValuePair<Vector2Int, Tile> entry in solidTiles) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                minimapTileMap.SetTile(location, solidTile);
        }
    }
}
