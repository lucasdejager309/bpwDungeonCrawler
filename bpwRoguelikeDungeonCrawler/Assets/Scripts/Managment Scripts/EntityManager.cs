using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityManager : Singleton<EntityManager>
{
    public List<Vector2Int> validPositions = new List<Vector2Int>();
    public Dictionary<Vector2Int, GameObject> entityDict = new Dictionary<Vector2Int, GameObject>();
    
    void Awake() {
        Instance = this;
        EventManager.AddListener("DUNGEON_GENERATED", FillValidPositionsDict);
        EventManager.AddListener("RELOAD_DUNGEON", DeleteEntities);
        EventManager.AddListener("PLAYER_TURN_FINISHED", OtherTurns);
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
        Debug.Log("vp " + validPositions.Count);
    }

    void OtherTurns() {
        StartCoroutine(DoActions());
    }

    IEnumerator DoActions() {
        List<GameObject> entities = new List<GameObject>();

        foreach(KeyValuePair<Vector2Int, GameObject> entity in entityDict) {
            if (entity.Value.gameObject.tag != "Player") {
                if (entity.Value.GetComponent<Entity>().TileInSight(GameObject.FindGameObjectWithTag("Player").transform.position)) {
                    entities.Add(entity.Value);
                }   
            }
        }

        if (entities.Count != 0) {
            int i = 0;
            bool next = true;
            while (i < entities.Count) {
                Task t = new Task(entities[i].GetComponent<Entity>().DoAction(), false);
                if (next) {
                    t.Start();
                    next = false;
                }
                t.Finished += delegate {
                    i++;
                    next = true;
                };
                yield return null;
            }   
        }
        EventManager.InvokeEvent("OTHERS_TURN_FINISHED"); 
    }
    
    void DeleteEntities() {
        foreach (KeyValuePair<Vector2Int, GameObject> entity in entityDict) {
            GameObject.Destroy(entity.Value);
        }

        entityDict.Clear();
        validPositions.Clear();
    }
}
