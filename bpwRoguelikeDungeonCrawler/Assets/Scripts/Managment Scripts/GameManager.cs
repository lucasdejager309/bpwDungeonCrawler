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
        player = Instantiate(playerPrefab, DungeonGen.Instance.SpawnPos, Quaternion.identity);
        EventManager.InvokeEvent("PLAYER_SPAWNED");
    }

    public Vector2Int GetPlayerPos() {
        return new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y);
    }
}
