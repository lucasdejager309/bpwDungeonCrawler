using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public GameObject playerPrefab;
    public GameObject player;

    void Awake() {
        Instance = this;
    }

    void Start()
    {   
        EventManager.AddListener("DUNGEON_GENERATED", SpawnPlayer);
        DungeonGen.Instance.GenerateDungeon();

        
    }

    void SpawnPlayer() {
        player = Instantiate(playerPrefab, (Vector2)DungeonGen.Instance.SpawnPos, Quaternion.identity);
        EventManager.InvokeEvent("PLAYER_SPAWNED");
    }

    //temp
    public Vector2Int GetPlayerPos() {
        player.GetComponent<Entity>().UpdatePosInDict();
        return EntityManager.Instance.entityDict[player.GetComponent<Entity>()];
    }

    //temp
    public void DrawPath(List<PathNode> nodes) {
        for (int i = 0; i < nodes.Count-1; i++) {
            Vector2 startPos = nodes[i].pos;
            startPos.x += 0.5f; startPos.y += 0.5f;
            Vector2 endPos = nodes[i+1].pos;
            endPos.x += 0.5f; endPos.y += 0.5f;

            Debug.DrawLine(startPos, endPos);
        }
    }
}
