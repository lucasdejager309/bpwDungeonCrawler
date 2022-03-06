using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventorySlot {
    public int amount;
    public Item item;
}

public class Inventory : MonoBehaviour
{
    private Dictionary <Item, int> items = new Dictionary<Item, int>();
    
    public Dictionary<Item, int> Items {
        get {return items;}
    }   
    public const int MAX_SLOTS = 9;

    void Start() {
        EventManager.AddListener("RELOAD_DUNGEON", ClearInventory);
    }

    public virtual bool AddItem(Item item) {
        if (items.ContainsKey(item)) {
            items[item]++;
        
            return true;

        } else if (!InventoryFull()) {
            items.Add(item, 1);
            
            return true;

        } else return false;

        
    }

    public virtual bool RemoveItem(Item item) {
        if (items.ContainsKey(item)) {
            items[item]--;

            if (items[item] <= 0) {
                items.Remove(item);
            }

            return true;
        }

        return false;
    }

    public int GetItemAmount(Item item) {
        if (items.ContainsKey(item)) {
            return items[item];
        } else return 0;
    }

    public void ClearInventory() {
        items.Clear();
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
    }

    private bool InventoryFull() {
        if (items.Count >= MAX_SLOTS) {
            return true;
        } else return false;
    }
}
