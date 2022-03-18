using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public bool loadFromSave;
    
    [SerializeField] private int currentLevel = 0;
    public DungeonSettings[] settings;
    public DungeonAppearance[] appearances;
    public Saving saving;
    [SerializeField] private int currentTurn = 0;
    public int CurrentTurn {
        get {
            return currentTurn;
        }
    }

    public enum Controlling {
        PLAYER,
        INVENTORY,
        INVENTORYCARD,
        AIM_POINTER,
        ESC_MENU
    }   
    public Controlling currentlyControling = Controlling.PLAYER;


    public GameObject playerPrefab;
    public GameObject player;
    public Minimap minimap;
    
    CameraFollowPlayer camera;

    void Awake() {
        Instance = this;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollowPlayer>();
    }

    void Start()
    {   EventManager.AddListener("DUNGEON_GENERATED", SpawnPlayer);
        EventManager.AddListener("ADD_TURN", AddTurn);
        EventManager.AddListener("TOGGLE_INVENTORY", ToggleInventory);
        EventManager.AddListener("INTERACT", Interact);
        EventManager.AddListener("TOGGLE_AIM", ToggleAimingPointer);
        EventManager.AddListener("ESC", Esc);
        EventManager.AddListener("SAVE", Save);
        EventManager.AddListener("LOAD", Load);

        EntityManager.Instance.entityDict.Clear();
        EntityManager.Instance.validPositions.Clear();

        DungeonGen.Instance.GenerateDungeon();
        if (loadFromSave) {
            Load();
        }
        

        UIManager.Instance.inventory.TogglePanel(false);
        UIManager.Instance.inventoryCard.TogglePanel(false);
        UIManager.Instance.escMenu.TogglePanel(false);
        UIManager.Instance.aimpointer.SetActive(false);
    }

    public void Save() { 
        saving.Save(currentLevel, player);

    }

    public void Load() {
        saving.GetSave();
        
        currentLevel = saving.save.currentLevel;
        DungeonGen.Instance.GenerateDungeon();

    }

    public DungeonSettings GetSettings() {
        if (currentLevel <= settings.Length) {
            return settings[currentLevel];
        }
        else {
            return settings[settings.Length];
        }
    }

    public DungeonAppearance GetAppearance() {
        return appearances[Random.Range(0, appearances.Length)];
    }

    public void NextLevel() {
        currentLevel++;
    }

    void Interact() {
        switch (currentlyControling) {
            case Controlling.PLAYER:
                StartCoroutine(player.GetComponent<Interact>().DoInteract(player.GetComponent<Player>().GetPos()));
                break;
            case Controlling.INVENTORY:
                UIManager.Instance.inventory.DoActionAtPointer();
                break;
            case Controlling.INVENTORYCARD:
                UIManager.Instance.inventoryCard.DoActionAtPointer();
                break;
            case Controlling.AIM_POINTER:
                if (player.GetComponent<RangedAttack>().HasAimOnTarget(UIManager.Instance.aimpointer.GetPos())) {
                    player.GetComponent<Player>().inputAllowed = false;
                    Esc();
                    Task t = new Task(player.GetComponent<RangedAttack>().DoAttack(UIManager.Instance.aimpointer.GetPos(), player.GetComponent<Player>().CalculateDamage(UIManager.Instance.aimpointer.itemToThrow), player.GetComponent<Entity>()));
                    
                    t.Finished += delegate {
                        EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                        UIManager.Instance.aimpointer.itemToThrow.DeleteItem();
                    };
                }
                break;
            case Controlling.ESC_MENU:
                UIManager.Instance.escMenu.DoActionAtPointer();
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

    void ToggleAimingPointer() { 
        SetControlTo(Controlling.AIM_POINTER);
    }

    void Esc() {        
        if (currentlyControling != Controlling.PLAYER) {
            SetControlTo(Controlling.PLAYER);
        } else {
            SetControlTo(Controlling.ESC_MENU);
        }
    }

    public void SetControlTo(Controlling control) {
        currentlyControling = control;

        camera.RemoveCameraFollow(UIManager.Instance.aimpointer.gameObject);

        switch (currentlyControling) {
            case Controlling.PLAYER:
                UIManager.Instance.escMenu.TogglePanel(false);
                UIManager.Instance.escMenu.SetPointer(0);
                UIManager.Instance.inventory.TogglePanel(false);
                UIManager.Instance.inventoryCard.TogglePanel(false);
                UIManager.Instance.inventory.SetPointer(0);
                UIManager.Instance.aimpointer.SetActive(false);
                
                break;
            case Controlling.INVENTORY:
                
                UIManager.Instance.inventory.TogglePanel(true);
                UIManager.Instance.inventoryCard.TogglePanel(false);
                UIManager.Instance.inventory.UpdateInventory();
                UIManager.Instance.aimpointer.SetActive(false);
                
                break;
            case Controlling.INVENTORYCARD:
                UIManager.Instance.inventory.UpdateCard();
                UIManager.Instance.inventoryCard.TogglePanel(true);
                UIManager.Instance.inventoryCard.SetPointer(0);
                UIManager.Instance.aimpointer.SetActive(false);

                break;
            case Controlling.AIM_POINTER:
                UIManager.Instance.inventory.TogglePanel(false);
                UIManager.Instance.inventoryCard.TogglePanel(false);
                UIManager.Instance.inventoryCard.SetPointer(0);
                UIManager.Instance.aimpointer.SetPos(player.GetComponent<Player>().GetPos());
                UIManager.Instance.aimpointer.SetActive(true);

                camera.SetCameraToFollow(UIManager.Instance.aimpointer.gameObject);

                break;
            case Controlling.ESC_MENU:
                UIManager.Instance.escMenu.TogglePanel(true);

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
                    player.GetComponent<Player>().UpdatePlayer(input);
                    break;

                case Controlling.INVENTORY:
                    UIManager.Instance.inventory.UpdatePointer(input);
                    break;

                case Controlling.INVENTORYCARD:
                    UIManager.Instance.inventoryCard.UpdatePointer(input);
                    break;

                case Controlling.AIM_POINTER:
                    UIManager.Instance.aimpointer.UpdatePos(input);
                    break;
                
                case Controlling.ESC_MENU:
                    UIManager.Instance.escMenu.UpdatePointer(input);
                    break;
            }
        }

        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void AddTurn() {
        currentTurn++;
    }

    void SpawnPlayer() {
        if (player == null) {
            player = Instantiate(playerPrefab, (Vector2)DungeonGen.Instance.SpawnPos, Quaternion.identity);
        } else {
            
            player.GetComponent<Entity>().SetPos(DungeonGen.Instance.SpawnPos);
            
        }

        if (loadFromSave) {
            saving.GetSave();

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            Player playerScript = player.GetComponent<Player>();

            inventory.equipSlots = saving.GetSlotsFromSave(player.GetComponent<PlayerInventory>().equipSlots);
            inventory.SetInventory(saving.GetItemsFromSave());
            
            player.GetComponent<Player>().SetHealth(saving.save.currentHealth);
            playerScript.SetStrength(saving.save.currentStrength);
            playerScript.SetInteligence(saving.save.currentInteligence);
        }

        camera.SetCameraToFollow(player);
        camera.SetCameraPos(DungeonGen.Instance.SpawnPos);
        minimap.SetDungeon();
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
