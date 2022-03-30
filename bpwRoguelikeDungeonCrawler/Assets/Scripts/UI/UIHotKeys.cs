using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HotKey {
    public KeyCode key;
    [SerializeField] Item item = null;
    public Item Item {
        get {
            return item;
        }
    }

    public void AssignItem(Item item) {
        this.item = item;
    }

    public void ClearItem() {
        item = null;
    }
}

public class UIHotKeys : MonoBehaviour
{
    public Image darkBackground;
    public Sprite emptySprite;
    public HotKey[] hotKeys;
    public GameObject[] hotKeyDisplays;
    
    void Start() {
        EventManager.AddListener("UI_UPDATE_INVENTORY", UpdateUI);
        darkBackground.enabled = false;
    }

    void UpdateUI() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        for (int i = 0; i < hotKeys.Length; i++) {
            Image image = hotKeyDisplays[i].transform.GetChild(0).GetComponent<Image>();
            Text text = hotKeyDisplays[i].transform.GetChild(1).GetComponent<Text>();
            
            if (hotKeys[i].Item != null && inventory.GetItemAmount(hotKeys[i].Item) > 0) {
                image.sprite = hotKeys[i].Item.itemSprite;
                int amount = inventory.GetItemAmount(hotKeys[i].Item);
                if (amount != 0) {
                    text.text = amount.ToString();
                } else text.text = "";
                
            } else {
                image.sprite = emptySprite;
                text.text = "";
            }
        }
    }

    public IEnumerator GetQuickSlotInput(Item itemToQuickSlot) {
        GameManager.Instance.SetControlTo(ControlMode.QUICK_SLOT);

        bool buttonPressed = false;
        while (!buttonPressed) {
            foreach (HotKey hotKey in hotKeys) {
                if (Input.GetKeyDown(hotKey.key)) {
                    Item item = itemToQuickSlot; //WHY DO I NEED THIS??
                    hotKey.AssignItem(item);
                    buttonPressed = true;
                    GameManager.Instance.SetControlTo(ControlMode.PLAYER);
                }
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    GameManager.Instance.SetControlTo(ControlMode.PLAYER);
                }
            }
            yield return null;
        }
    }


    void Update() {
        foreach(HotKey hotKey in hotKeys) {
            if (Input.GetKeyDown(hotKey.key)) {
                PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
                
                if (hotKey.Item != null) {
                    inventory.GetInventoryItem(hotKey.Item).item.Use();
                    if (inventory.GetItemAmount(hotKey.Item) <= 0) {

                    }
                }
            }
        }
    }
}
