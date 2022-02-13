using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityManager : Singleton<EntityManager>
{
    public List<Vector2Int> validPositions;
    public Dictionary<Vector2Int, GameObject> entityPositions = new Dictionary<Vector2Int, GameObject>();
    
    void Awake() {
        Instance = this;
        EventManager.AddListener("DUNGEON_GENERATED", GetValidPositions);
        EventManager.AddListener("RELOAD_DUNGEON", DeleteEntities);
    }

    void GetValidPositions() {
        validPositions = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, Tile> tile in DungeonGen.Instance.floorTilelayer.tileDictionary) {
            validPositions.Add(tile.Key);
        }
        foreach (KeyValuePair<Vector2Int, Tile> tile in DungeonGen.Instance.wallTilelayer.tileDictionary) {
            if (validPositions.Contains(tile.Key)) {
                validPositions.Remove(tile.Key);
            }         
        }

        foreach (Room room in DungeonGen.Instance.roomList) {
            for (int x = room.position.x; x < room.position.x+room.size.x; x++) {
                for (int y = room.position.y; y < room.position.y+room.size.y; y++) {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (validPositions.Contains(pos)) {
                        room.validSpawnPositions.Add(pos);
                    }
                }
            }
        }
    }
    
    void DeleteEntities() {
        foreach (KeyValuePair<Vector2Int, GameObject> entityPos in entityPositions) {
            GameObject.Destroy(entityPos.Value);
        }

        entityPositions.Clear();
        validPositions.Clear();
    }
}
