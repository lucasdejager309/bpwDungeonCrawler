using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    [SerializeField]
    private int currentTurn = 0;
    public int CurrentTurn {
        get {
            return currentTurn;
        }
    }
    
    public GameObject playerPrefab;
    public GameObject player;

    public enum GameState {
        PLAYING,
        IN_INVENTORY,
    }

    public GameState currentGameState = GameState.PLAYING;

    void Awake() {
        Instance = this;
    }

    void Start()
    {   
        EventManager.AddListener("DUNGEON_GENERATED", SpawnPlayer);
        EventManager.AddListener("ADD_TURN", AddTurn);
        EventManager.AddListener("TOGGLE_INVENTORY", ToggleInventory);
        DungeonGen.Instance.GenerateDungeon();
    }

    void ToggleInventory() {
        switch (currentGameState) {
            case GameState.PLAYING:
                currentGameState = GameState.IN_INVENTORY;
                UIManager.Instance.ToggleInventory();
                break;
            case GameState.IN_INVENTORY:
                currentGameState = GameState.PLAYING;
                UIManager.Instance.ToggleInventory();
                player.GetComponent<PlayerInventory>().SetInventoryPointer(0);
                EventManager.InvokeEvent("UI_UPDATE_INVENTORY_POINTER");
                break;
        }
    }

    void Update() {
        switch (currentGameState) {
            case GameState.PLAYING:
                player.GetComponent<Player>().UpdatePlayer(GetInput());
                break;

            case GameState.IN_INVENTORY:
                
                player.GetComponent<PlayerInventory>().UpdateInventoryPointer(GetInput());
                break;
        }
    }

    void AddTurn() {
        currentTurn++;
    }

    void SpawnPlayer() {
        player = Instantiate(playerPrefab, (Vector2)DungeonGen.Instance.SpawnPos, Quaternion.identity);
        EventManager.InvokeEvent("PLAYER_SPAWNED");
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
    }

    Vector2Int GetInput() {
        Vector2Int input = new Vector2Int();

        if (Input.GetKeyDown(KeyCode.W)) {
            input.y = 1;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            input.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            input.x = -1;
        }
        if (Input.GetKeyDown
         (KeyCode.D)) {
            input.x = 1;
        }

        return input;
    }

    //temp
    public void DrawPath(List<PathNode> nodes) {
        for (int i = 0; i < nodes.Count-1; i++) {
            Vector2 startPos = nodes[i].pos;
            startPos.x += 0.5f; startPos.y += 0.5f;
            Vector2 endPos = nodes[i+1].pos;
            endPos.x += 0.5f; endPos.y += 0.5f;

            Debug.DrawLine(startPos, endPos, Color.red, 1f);
        }
    }
}
