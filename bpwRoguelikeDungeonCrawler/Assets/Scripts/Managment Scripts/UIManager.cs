using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Inventory")]
    public GameObject inventoryObject;
    public GameObject inventoryPointer;
    public GameObject[] inventorySlots;
    

    void Awake() {
        Instance = this;

        EventManager.AddListener("UI_WAIT", UpdateWait);
        EventManager.AddListener("UI_UPDATE_INVENTORY", UpdateInventory);
        EventManager.AddListener("UI_UPDATE_INVENTORY_POINTER", UpdateInventoryPointer);
        EventManager.AddListener("INTERACT", ToggleItemCard);


        foreach(GameObject slot in inventorySlots) {
            slot.transform.GetChild(0).GetComponent<Image>().enabled = false;
            slot.transform.GetChild(1).GetComponent<Text>().enabled = false;
        }
    }

    void UpdateInventoryPointer() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        int pointerIndex = inventory.pointerIndex;

        inventoryPointer.transform.position = inventorySlots[pointerIndex].transform.position;
        
        if (pointerIndex < inventory.Items.Count) {
            inventoryPointer.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = inventory.Items[pointerIndex].item.itemName;
        }
    }

    void ToggleItemCard() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        
        if (inventory.pointerIndex < inventory.Items.Count) {
            inventory.inputAllowed = !inventory.inputAllowed;
            if (GameManager.Instance.currentGameState == GameManager.GameState.IN_INVENTORY) {
                inventoryPointer.transform.GetChild(0).gameObject.SetActive(!inventoryPointer.transform.GetChild(0).gameObject.activeSelf);
            }
        }
    }

    void UpdateWait() {
        Image wait = GameObject.FindGameObjectWithTag("UIWait").GetComponent<Image>();
        wait.enabled = !wait.enabled;
    }

    public void ToggleInventory() {
        GameManager.Instance.player.GetComponent<PlayerInventory>().inputAllowed = true;
        inventoryPointer.transform.GetChild(0).gameObject.SetActive(false);
        inventoryObject.SetActive(!inventoryObject.activeSelf);
    }

    void UpdateInventory() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();

        int index = 0;
        foreach (InventoryItem item in inventory.Items) {
            if (item != null) {
                inventorySlots[index].transform.GetChild(0).GetComponent<Image>().sprite = item.item.itemSprite;
                inventorySlots[index].transform.GetChild(1).GetComponent<Text>().text = item.amount.ToString();
                inventorySlots[index].transform.GetChild(0).GetComponent<Image>().enabled = true;
                inventorySlots[index].transform.GetChild(1).GetComponent<Text>().enabled = true;
            }
            index++;
        }

        for (int i = 0; i < inventorySlots.Length; i++) {
            if (i >= index) {
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                inventorySlots[i].transform.GetChild(1).GetComponent<Text>().enabled = false;
            }
        }

    }
}
