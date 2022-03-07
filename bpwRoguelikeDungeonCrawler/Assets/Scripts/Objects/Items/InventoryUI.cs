using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryObject;
    public GameObject inventoryPointer;
    public GameObject pointerCard;
    public UISlot[] inventorySlots;
    public UISlot[] equipedSlots;

    void Start() {
        EventManager.AddListener("TOGGLE_INVENTORY", ToggleInventory);
        EventManager.AddListener("ESC", ToggleInventory);
        EventManager.AddListener("UI_UPDATE_INVENTORY", UpdateInventory);
        EventManager.AddListener("UI_UPDATE_INVENTORY_POINTER", UpdateInventoryPointer);
        EventManager.AddListener("INTERACT", ToggleItemCardOn);
        EventManager.AddListener("ESC", ToggleItemCardOff);

        foreach(UISlot slot in inventorySlots) {
            slot.gameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
            slot.gameObject.transform.GetChild(1).GetComponent<Text>().enabled = false;
        }
    }


    //set position of pointer
    void UpdateInventoryPointer() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        int pointerIndex = inventory.pointerIndex;


        if (pointerIndex < inventory.MAX_SLOTS) {
            inventoryPointer.transform.position = inventorySlots[pointerIndex].gameObject.transform.position;
        } else {
            foreach (UISlot slot in equipedSlots) {
                if (inventory.equipSlots[pointerIndex-inventory.MAX_SLOTS].slotID == slot.slotID) {
                    inventoryPointer.transform.position = slot.gameObject.transform.position;
                }
            }
            
        }
    }

    void ToggleItemCardOn() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        int pointerIndex = inventory.pointerIndex;
        
        if (inventory.GetItem(pointerIndex) != null) {
            if (GameManager.Instance.currentGameState == GameManager.GameState.IN_INVENTORY) {
                
                inventoryPointer.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = inventory.GetItem(pointerIndex).itemName + " X" + inventory.GetItemAmount(inventory.GetItem(pointerIndex));
                inventory.inputAllowed = false;
                inventoryPointer.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    void ToggleItemCardOff() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        int pointerIndex = inventory.pointerIndex;
        
        if (GameManager.Instance.currentGameState == GameManager.GameState.IN_INVENTORY) {
            inventoryPointer.transform.GetChild(0).gameObject.SetActive(false);
            inventory.inputAllowed = true;
        }
    }

    public void ToggleInventory() {
        GameManager.Instance.player.GetComponent<PlayerInventory>().inputAllowed = true;
        inventoryPointer.transform.GetChild(0).gameObject.SetActive(false);
        inventoryObject.SetActive(!inventoryObject.activeSelf);
    }

    void UpdateInventory() {
        //i swear to god this code was written by a monkey with a typewriter
        
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();

        int index = 0;
        foreach (InventoryItem item in inventory.Items) {
            if (item != null) {
                inventorySlots[index].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = item.item.itemSprite;
                inventorySlots[index].gameObject.transform.GetChild(1).GetComponent<Text>().text = item.amount.ToString();
                inventorySlots[index].gameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
                inventorySlots[index].gameObject.transform.GetChild(1).GetComponent<Text>().enabled = true;
            }
            index++;
        }

        for (int i = 0; i < inventorySlots.Length; i++) {
            if (i >= index) {
                inventorySlots[i].gameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
                inventorySlots[i].gameObject.transform.GetChild(1).GetComponent<Text>().enabled = false;
            }
        }

        index = 0;
        foreach (EquipSlot slot in inventory.equipSlots) {
            if (slot.item != null) {
                equipedSlots[index].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = slot.item.itemSprite;
                equipedSlots[index].gameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
            }
            index++;
        }

        for (int i = 0; i < equipedSlots.Length; i++) {
            if (i >= index) {
                equipedSlots[index].gameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

}
