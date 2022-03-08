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

    public enum Controlling {
        PLAYER,
        INVENTORY,
        INVENTORYCARD
    }

    public Controlling currentlyControling = Controlling.PLAYER;

    void Awake() {
        Instance = this;
    }

    void Start()
    {   EventManager.AddListener("DUNGEON_GENERATED", SpawnPlayer);
        EventManager.AddListener("ADD_TURN", AddTurn);
        EventManager.AddListener("TOGGLE_INVENTORY", ToggleInventory);
        EventManager.AddListener("INTERACT", Interact);
        DungeonGen.Instance.GenerateDungeon();

        UIManager.Instance.inventory.TogglePanel(false);
        UIManager.Instance.inventoryCard.TogglePanel(false);
    }

    void Interact() {
        switch (currentlyControling) {
            case Controlling.PLAYER:
                StartCoroutine(player.GetComponent<Interact>().DoInteract(player.GetComponent<Player>().GetPos()));
                EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                break;
            case Controlling.INVENTORY:
                UIManager.Instance.inventory.DoActionAtPointer();
                break;
            case Controlling.INVENTORYCARD:
                UIManager.Instance.inventoryCard.DoActionAtPointer();
                break;
        }
    }

    void ToggleInventory() {
        switch(currentlyControling) {
            case(Controlling.PLAYER):
                SetControlTo(Controlling.INVENTORY);
                break;
            case(Controlling.INVENTORY):
                SetControlTo(Controlling.PLAYER);
                break;
            case(Controlling.INVENTORYCARD):
                SetControlTo(Controlling.INVENTORY);
                break;
            default:
                break;
        }
    }

    public void SetControlTo(Controlling control) {
        currentlyControling = control;

        switch (currentlyControling) {
            case Controlling.PLAYER:
                UIManager.Instance.inventory.TogglePanel(false);
                UIManager.Instance.inventoryCard.TogglePanel(false);
                UIManager.Instance.inventory.SetPointer(0);
                
                break;
            case Controlling.INVENTORY:
                
                UIManager.Instance.inventory.TogglePanel(true);
                UIManager.Instance.inventoryCard.TogglePanel(false);
                UIManager.Instance.inventory.UpdateInventory();
                
                break;
            case Controlling.INVENTORYCARD:
                UIManager.Instance.inventory.UpdateCard();
                UIManager.Instance.inventoryCard.TogglePanel(true);
                UIManager.Instance.inventoryCard.SetPointer(0);

                break;
            default:
                break;
        }
    }

    void Update() {
        Vector2Int input = GetInput();
        if (input != new Vector2Int(0, 0)) {
            switch (currentlyControling) {
                case Controlling.PLAYER:
                    player.GetComponent<Player>().UpdatePlayer(GetInput());
                    break;

                case Controlling.INVENTORY:
                    UIManager.Instance.inventory.UpdatePointer(GetInput());
                    break;

                case Controlling.INVENTORYCARD:
                    UIManager.Instance.inventoryCard.UpdatePointer(GetInput());
                    break;
            }
        }
        
    }

    void AddTurn() {
        currentTurn++;
    }

    void SpawnPlayer() {
        player = Instantiate(playerPrefab, (Vector2)DungeonGen.Instance.SpawnPos, Quaternion.identity);
        EventManager.InvokeEvent("PLAYER_SPAWNED");
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
