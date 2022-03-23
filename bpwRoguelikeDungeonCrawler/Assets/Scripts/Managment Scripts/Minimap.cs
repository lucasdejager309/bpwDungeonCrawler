using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Minimap : MonoBehaviour
{
    public Tilemap minimapTileMap;
    

    public void SetDungeon() {
        DungeonAppearance appearance = GameManager.Instance.GetAppearance();
        minimapTileMap.ClearAllTiles();
        
        Dictionary<Vector2Int, Tile> floorTiles = DungeonGen.Instance.floorTileDictionary;
        Dictionary<Vector2Int, Tile> solidTiles = DungeonGen.Instance.solidTileDictionary;

        foreach(KeyValuePair<Vector2Int, Tile> entry in floorTiles) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                minimapTileMap.SetTile(location, appearance.floorTile);
        }

        foreach(KeyValuePair<Vector2Int, Tile> entry in solidTiles) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                minimapTileMap.SetTile(location, appearance.solidTile);
        }
    }
}
