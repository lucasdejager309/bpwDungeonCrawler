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
    }

    //temp DRAWS NICE LITTLE CROSSES AT ENTITYDICT VALUES
    void Update() {
        foreach (Vector2Int pos in entityDict.Values) {
            Vector2 startPos = new Vector2(pos.x, pos.y);
            Vector2 endPos = new Vector2(pos.x+1f, pos.y+1f);
            
            Debug.DrawLine(startPos, endPos, Color.green);

            startPos = new Vector2(pos.x+1, pos.y);
            endPos = new Vector2(pos.x, pos.y+1f);

            Debug.DrawLine(startPos, endPos, Color.green);
        }
    }

    public void ClearEntityDict() {
        List<Entity> entities = new List<Entity>(entityDict.Keys);
        entityDict.Clear();
        foreach(var key in entities) {
            key.DeleteEntity();
        }
    }

    void FillValidPositionsDict() {
        ClearValidPositions();
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
    }

    public void UpdatePos(Entity entity, Vector2Int entityPos) {
        //Vector2Int entityPos = entity.GetPos();
        
        if (entityDict.ContainsKey(entity) && entityDict[entity] != entityPos) {
                
            entityDict.Remove(entity);
            entityDict.Add(entity, entityPos);

        } else if(!entityDict.ContainsKey(entity)) {

            entityDict.Add(entity, entityPos);
        }
    }

    public Entity EntityAtPos(Vector2Int pos) {
        foreach (var kv in entityDict) {
            if (kv.Value == pos) {                
                return kv.Key;
            }
        }

        return null;
    }

    public Vector2Int PosOfEntity(Entity entity) {
        return entityDict[entity];
    }

    public static List<Vector2Int> GetNeighbours(Vector2Int pos, bool getCorners, bool includeSelf) {
        List<Vector2Int> positionsToReturn = new List<Vector2Int>();
        
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0 && !includeSelf) {
                    continue;
                }
                if (!getCorners && Mathf.Abs(x) == Mathf.Abs(y)) {
                    continue;
                }
                
                positionsToReturn.Add(pos + new Vector2Int(x,y));
            }
        }

        return positionsToReturn;
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

    public GameObject SpawnEntity(Vector2Int pos, GameObject objectToSpawn) {
        GameObject spawnedObject = Instantiate(objectToSpawn, new Vector3(pos.x, pos.y, -2), Quaternion.identity);
        entityDict.Add(spawnedObject.gameObject.GetComponent<Entity>(), pos);
        return spawnedObject;
    }

    void ClearValidPositions() {
        validPositions.Clear();
    }
}
