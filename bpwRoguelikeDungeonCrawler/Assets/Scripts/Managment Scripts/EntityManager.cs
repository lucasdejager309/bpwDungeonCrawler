using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityManager : Singleton<EntityManager>
{
    public List<Vector2Int> validPositions = new List<Vector2Int>();
    public Dictionary<Entity, Vector2Int> entityDict = new Dictionary<Entity, Vector2Int>();
    
    void Awake() {
        Instance = this;
        EventManager.AddListener("DUNGEON_GENERATED", FillValidPositionsDict);
        EventManager.AddListener("RELOAD_DUNGEON", DeleteEntities);
    }

    void FillValidPositionsDict() {
        foreach (KeyValuePair<Vector2Int, Tile> tile in DungeonGen.Instance.floorTilelayer.tileDictionary) {
            validPositions.Add(tile.Key);
        }
        foreach (KeyValuePair<Vector2Int, Tile> tile in DungeonGen.Instance.solidTileLayer.tileDictionary) {
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

        Debug.Log(validPositions.Count);
    }

    public Dictionary<Vector2Int, GameObject> SpawnByDensity(SpawnableObject[] prefabs, float minDensity, float maxDensity) {
        Dictionary<Vector2Int, GameObject> objectsToReturn = new Dictionary<Vector2Int, GameObject>();
        foreach (Room room in DungeonGen.Instance.roomList) {
            if (!room.spawnRoom) {
                room.enemyDensity = Random.Range(minDensity, maxDensity);

                foreach(Vector2Int spawnPos in room.validSpawnPositions) {
                    SpawnableObject pickedObject = PickRandom.PickRandomObject(prefabs);
                    if ((!pickedObject.wallAdjacent || room.IsWallAdjacent(spawnPos)) && !entityDict.ContainsValue(spawnPos)) {
                        if (Random.Range(0, 101) < room.enemyDensity) {    
                            objectsToReturn.Add(spawnPos, pickedObject.gameObject);
                        }
                    } 
                }
            }
        }
        return objectsToReturn;
    }

    public Dictionary<Vector2Int, GameObject> SpawnByNumber(SpawnableObject[] prefabs, float minDensity, float maxDensity) {
        Dictionary<Vector2Int, GameObject> objectsToReturn = new Dictionary<Vector2Int, GameObject>();
        
        foreach(Room room in DungeonGen.Instance.roomList) {
            int amountOfObjects = (int)Random.Range(minDensity, maxDensity);
            for (int i = 0; i < amountOfObjects; i++) {
                SpawnableObject objectToAdd = PickRandom.PickRandomObject(prefabs);

                Vector2Int spawnPos = new Vector2Int();
                bool posFound = false;
                while (!posFound) {
                    spawnPos = room.GetRandomPosInRoom(objectToAdd.wallAdjacent);
                    if (!entityDict.ContainsValue(spawnPos)) {
                        posFound = true;
                    }
                }

                objectsToReturn.Add(spawnPos, objectToAdd.gameObject);
            }
        }

        return objectsToReturn;
    }    
    public void SpawnEntities(Dictionary<Vector2Int, GameObject> objects) {
        foreach(KeyValuePair<Vector2Int, GameObject> objectToSpawn in objects) {
            SpawnEntity(objectToSpawn.Key, objectToSpawn.Value);
        }
    }

    public void SpawnEntity(Vector2Int pos, GameObject objectToSpawn) {
        GameObject spawnedObject = Instantiate(objectToSpawn, new Vector3(pos.x, pos.y, 1), Quaternion.identity);
        entityDict.Add(spawnedObject.gameObject.GetComponent<Entity>(), pos);
    }

    void DeleteEntities() {
        foreach (KeyValuePair<Entity, Vector2Int> entity in entityDict) {
            entity.Key.DeleteEntity();
        }

        entityDict.Clear();
        validPositions.Clear();
    }
}
