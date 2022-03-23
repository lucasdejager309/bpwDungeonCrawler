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
    public GameObject player;
    public Minimap minimap;
    
    public CameraFollowPlayer cameraFollow;

    [Header("Controllers")]
    public ControlMode currentlyControlling;
    public ControlObject[] controllers;

    DungeonAppearance appearance = null;

    void Awake() {
        Instance = this;
    }

    void Start() {
        //GAME EVENTS
        EventManager.AddListener("DUNGEON_GENERATED", SpawnPlayer);
        EventManager.AddListener("DUNGEON_GENERATED", SpawnObjects);

        //INPUT EVENTS
        EventManager.AddListener("INTERACT", Interact); //SPACE
        EventManager.AddListener("ESC", Esc); //ESCAPE
        EventManager.AddListener("TOGGLE_INVENTORY", ToggleInventory); //E

        //SETUP CAMERA
        cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollowPlayer>();

        //LOAD DUNGEON
        if (loadFromSave) {
            Load();
        } else {
            DungeonGen.Instance.GenerateDungeon(GetAppearance(), GetSettings());
        }

        SetControlTo(ControlMode.PLAYER);

        //TURN OFF UI ELEMENTS
        UIManager.Instance.inventory.TogglePanel(false);
        UIManager.Instance.inventoryCard.TogglePanel(false);
        UIManager.Instance.escMenu.TogglePanel(false);
        UIManager.Instance.aimpointer.SetActive(false);
        UIManager.Instance.bigAnnouncementText.gameObject.SetActive(false);
    }

    public void Save() { 
        saving.Save(currentLevel, player);
    }

    public void Load() {
        saving.GetSave();
        
        currentLevel = saving.save.currentLevel;
        DungeonGen.Instance.GenerateDungeon(GetAppearance(), GetSettings());

    }

    public DungeonSettings GetSettings() {
        if (currentLevel < settings.Length) {
            return settings[currentLevel];
        }
        else {
            return settings[settings.Length-1];
        }
    }

    public DungeonAppearance GetAppearance() {
        if (appearance != null) {
            return appearance;
        } else {
            appearance = appearances[Random.Range(0, appearances.Length)];
            return appearance;
        }
        
    }

    public void NextLevel() {
        currentLevel++;
    }

    public void AddTurn() {
        currentTurn++;
    }

    public void SetControlTo(ControlMode mode) {
        currentlyControlling = mode;
        
        GetControlObject(mode).SetControlTo();
        
        foreach (ControlObject controller in controllers) {
            if (currentlyControlling == ControlMode.INVENTORYCARD && controller.mode == ControlMode.INVENTORY) {
                continue;
            }
            if (controller.mode != currentlyControlling) {
                Debug.Log(controller);
                controller.LoseControl();
            }
        }
        
    }

    void SpawnPlayer() {
        UIManager.Instance.DisplayAnnouncement("Level " + (currentLevel+1), 1);
        LogText.Instance.Log("You descended to level " + (currentLevel+1));
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
            playerScript.SetInteligence(saving.save.currentInteligence);
        }

        cameraFollow.SetCameraToFollow(player);
        cameraFollow.SetCameraPos(player.GetComponent<Player>().GetPos());
        minimap.SetDungeon();
        EventManager.InvokeEvent("PLAYER_SPAWNED");
    }

    void SpawnObjects() {
        DungeonSettings settings = GetSettings();

        Dictionary<Vector2Int, GameObject> objectsToSpawn = new Dictionary<Vector2Int, GameObject>();
        objectsToSpawn = EntityManager.Instance.SpawnByDensity(settings.interactableObjects, settings.interactableObjectsDensityRange.min, settings.interactableObjectsDensityRange.max);
        EntityManager.Instance.SpawnEntities(objectsToSpawn);
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

    public void ToggleAimingPointer() {
        SetControlTo(ControlMode.AIM_POINTER);
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

    void Interact() {
        GetControlObject(currentlyControlling).Interact();
    }

    void Update() {
        //UGLY FIX (HEALTHBAR DOES UPDATE IN EDITOR BUT NOT IN BUILD???? I DONT EVEN KNOW WHY THIS FIXES IT)
        EventManager.InvokeEvent("UI_UPDATE_HEALTH");

        //PASS INPUT TO OBJECT CURRENTLY IN CONTROL
        Vector2Int input = GetInput();
        if (input != new Vector2Int(0, 0)) {
            GetControlObject(currentlyControlling).UpdateControl(input);
        }

        //IN CASE PLAYER LOST
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
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
