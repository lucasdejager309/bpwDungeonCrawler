using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager> {
    public GameObject playerPrefab;
    public GameObject player;

    void Start()
    {   
        EventManager.AddListener("DUNGEON_GENERATED", SpawnPlayer);
        EventManager.AddListener("RELOAD_DUNGEON", DestroyPlayer);
        DungeonGen.Instance.GenerateDungeon();
    }

    void SpawnPlayer() {
        player = Instantiate(playerPrefab, DungeonGen.Instance.SpawnPos, Quaternion.identity);
    }

    void DestroyPlayer() {
        GameObject.Destroy(player);
    }
}
