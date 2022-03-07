using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem {
    public InventoryItem(Item item) {
        this.item = item;
        amount = 1;
    }
    public int amount;
    public Item item;
}

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> items = new List<InventoryItem>();
    
    public List<InventoryItem> Items {
        get {return items;}
    }   
    public const int COLUMN_SIZE = 3;
    public const int ROW_SIZE = 3;

    public int MAX_SLOTS {
        get {
            return COLUMN_SIZE*ROW_SIZE;
        }
    }

    void Start() {
        EventManager.AddListener("RELOAD_DUNGEON", ClearInventory);
    }

    public virtual bool AddItem(Item item) {
        foreach (InventoryItem slot in items) {
            if (slot.item == item && slot.item.stackAble) {
                slot.amount++;
                return true;
            }
        }

        if (!InventoryFull()) {
            items.Add(new InventoryItem(item));
            return true;
        }

        return false;
    }

    public virtual bool RemoveItem(Item item) {
        foreach (InventoryItem slot in items) {
            if (slot.item = item) {
                slot.amount--;
            }

            if (slot.amount <= 0) {
                items.Remove(slot);
            }

            return true;
        }

        return false;
    }

    public int GetItemAmount(Item item) {
        foreach (InventoryItem slot in Items) {
            if (slot.item == item) {
                return slot.amount;
            }
        }

        return 0;
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

    public virtual Item GetItem(int index) {
        if (index < Items.Count) {
            return Items[index].item;
        } else return null;
    }
}
