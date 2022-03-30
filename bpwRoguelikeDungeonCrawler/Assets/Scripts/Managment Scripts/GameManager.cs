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

    public GameObject playerPrefab;
    public GameObject droppedItemPrefab;
    public GameObject player;
    public Minimap minimap;
    
    public CameraFollowPlayer cameraFollow;

    [Header("Controllers")]
    public ControlMode currentlyControlling;
    public ControlObject[] controllers;

    void Awake() {
        Instance = this;
    }

    void Start() {
        //GAME EVENTS
        EventManager.AddListener("DUNGEON_GENERATED", SpawnEntities);
        EventManager.AddListener("DUNGEON_GENERATED", HideUI);

        //INPUT EVENTS
        EventManager.AddListener("INTERACT", Interact); //SPACE
        EventManager.AddListener("ESC", Esc); //ESCAPE
        EventManager.AddListener("TOGGLE_INVENTORY", ToggleInventory); //E
        EventManager.AddListener("SKIP_LEVEL", NextLevel);
        EventManager.AddListener("INSPECT", Inspect);

        //SETUP CAMERA
        cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollowPlayer>();

        //LOAD DUNGEON
        if (loadFromSave) {
            Load();
        } else {
            currentLevel++;
            DungeonGen.Instance.GenerateDungeon(GetAppearance(), GetSettings());
        }
    }

    public void Save() { 
        saving.Save(currentLevel, player);
    }

    public void Load() {
        saving.GetSave();
        
        currentLevel = saving.save.currentLevel;
        DungeonGen.Instance.GenerateDungeon(GetAppearance(), GetSettings());

    }
    
    public void NewGame() {
        player.GetComponent<PlayerInventory>().ResetInventory();
        player.GetComponent<Player>().ResetStats();
        SetControlTo(ControlMode.PLAYER);
        DungeonGen.Instance.GenerateDungeon(GetAppearance(), GetSettings());
    }

    public DungeonSettings GetSettings() {
        if (currentLevel < settings.Length) {
            return settings[currentLevel-1];
        }
        else {
            return settings[settings.Length-1];
        }
    }

    public DungeonAppearance GetAppearance() {
         if (currentLevel < appearances.Length) {
            return appearances[currentLevel-1];
        }
        else {
            return appearances[appearances.Length-1];
        }
        
    }

    public void NextLevel() {
        currentLevel++;
        DungeonGen.Instance.GenerateDungeon(GameManager.Instance.GetAppearance(), GameManager.Instance.GetSettings());
    }

    void Inspect() {
        SetControlTo(ControlMode.INSPECT);
    }

    public void AddTurn() {
        currentTurn++;
    }

    public void SetControlTo(ControlMode mode) {
        currentlyControlling = mode;

        foreach (ControlObject controller in controllers) {
            //I WANT BOTH INVENTORY AND INVENTORY CARD VISIBLE AT SAME TIME
            if (currentlyControlling == ControlMode.INVENTORYCARD && controller.mode == ControlMode.INVENTORY) {
                continue;
            }

            if (controller.mode != currentlyControlling) {
                controller.LoseControl();
            }
        }

        GetControlObject(mode).SetControlTo();
        
    }

    void HideUI() {
        UIManager.Instance.inventory.TogglePanel(false);
        UIManager.Instance.inventoryCard.TogglePanel(false);
        UIManager.Instance.escMenu.TogglePanel(false);
        UIManager.Instance.aimpointer.SetActive(false);
        UIManager.Instance.deathMenu.TogglePanel(false);
        UIManager.Instance.hotKeys.darkBackground.enabled = false;
        UIManager.Instance.hotKeys.promptText.enabled = false;
        UIManager.Instance.inspectPanel.TogglePanel(false);
    }

    void SpawnPlayer() {
        LogText.Instance.Log("You descended to level " + (currentLevel));
        if (player == null) {
            player = Instantiate(playerPrefab, (Vector2)DungeonGen.Instance.playerSpawnPos, Quaternion.identity);
        } else {
            player.GetComponent<Entity>().SetPos(DungeonGen.Instance.playerSpawnPos);
            
        }

        if (loadFromSave) {
            saving.GetSave();

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            Player playerScript = player.GetComponent<Player>();

            inventory.equipSlots = saving.GetSlotsFromSave(player.GetComponent<PlayerInventory>().equipSlots);
            
            inventory.SetInventory(saving.GetItemsFromSave());
            
            player.GetComponent<Player>().SetHealth(saving.save.currentHealth);
            playerScript.SetStrength(saving.save.currentStrength);
            playerScript.SetIntelligence(saving.save.currentIntelligence);
            
            SetControlTo(ControlMode.PLAYER);
        }

        cameraFollow.SetCameraToFollow(player);
        cameraFollow.SetCameraPos(player.GetComponent<Player>().GetPos());
        minimap.SetDungeon();

        Save();
    }

    void SpawnEntities() {
        SpawnRequiredObjects();
        SpawnPlayer();
        EnemyManager.Instance.SpawnEnemies();
        InteractableObjectsManager.Instance.SpawnObjects();
        PopulateRequiredLootItems();
    }

    void SpawnRequiredObjects() {
        foreach (RequiredObject obj in GetSettings().requiredObjects) {
            for (int i = 0; i < obj.amount; i++) {
                Vector2Int spawnPos = DungeonGen.Instance.GetRandomRoom().GetRandomPosInRoom(obj.spawnableObj.wallAdjacent);
            
                if (obj.spawnableObj.gameObject.GetComponent<Enemy>()) {
                    EnemyManager.Instance.SpawnEnemy(spawnPos, obj.spawnableObj.gameObject);
                } else EntityManager.Instance.SpawnEntity(spawnPos, obj.spawnableObj.gameObject);
            }
        }
    }

    void PopulateRequiredLootItems() {
        List<Entity> entities = new List<Entity>(EntityManager.Instance.entityDict.Keys);
        foreach (InventoryItem item in GetSettings().requiredItems) {
            for (int i = 0; i < item.amount; i++) {
                Entity entity = entities[0];

                while (entity.GetComponent<DropItems>() == null) {
                    entity = entities[Random.Range(0, entities.Count)];
                }

                entity.GetComponent<DropItems>().privateTable.Add(item.item);
            }
        }
    }

    public void Esc() {
        if (currentlyControlling != ControlMode.PLAYER) {
            SetControlTo(ControlMode.PLAYER);
        } else {
            SetControlTo(ControlMode.ESC_MENU);
        }
    }

    void ToggleInventory() {
        if (currentlyControlling == ControlMode.PLAYER || currentlyControlling == ControlMode.INVENTORYCARD) {
            SetControlTo(ControlMode.INVENTORY);
        }
    }

    void Interact() {
        AudioManager.Instance.PlaySound("test");
        GetControlObject(currentlyControlling).Interact();
    }

    void Update() {
        if (UIManager.Instance.healthSlider.GetValue() != player.GetComponent<Player>().Health) {
            EventManager.InvokeEvent("UI_UPDATE_HEALTH");
        }
        

        //PASS INPUT TO OBJECT CURRENTLY IN CONTROL
        Vector2Int input = InputManager.Instance.GetInput(new Axis(KeyCode.A, KeyCode.D), new Axis(KeyCode.S, KeyCode.W), true);
        if (input != new Vector2Int(0, 0)) {
            GetControlObject(currentlyControlling).UpdateControl(input);
        }

        if (player.GetComponent<Player>().inputAllowed) {
            InputManager.Instance.inputObject.GetInput();
        }
        
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void DrawCross(Vector2Int pos, float time, Color color) {
            Vector2 startPos = new Vector2(pos.x, pos.y);
            Vector2 endPos = new Vector2(pos.x+1f, pos.y+1f);
            
            Debug.DrawLine(startPos, endPos, color, time);

            startPos = new Vector2(pos.x+1, pos.y);
            endPos = new Vector2(pos.x, pos.y+1f);

            Debug.DrawLine(startPos, endPos, color, time);
    }
    
    ControlObject GetControlObject(ControlMode mode) {
         foreach (ControlObject controller in controllers) {
            if (controller.mode == mode) {
                return controller;
            }
         }

         return null;
    }  
}
