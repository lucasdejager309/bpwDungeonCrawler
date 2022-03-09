using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem {
    public InventoryItem(Item item, int amount) {
        this.item = item;
        this.amount = amount;
    }
    public int amount;
    public Item item;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    
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

    public GameObject droppedItemPrefab;

    void Start() {
        EventManager.AddListener("RELOAD_DUNGEON", ClearInventory);
    }

    public virtual bool AddItem(Item item, int amount = 1) {
        foreach (InventoryItem slot in items) {
            if (slot.item == item && slot.item.stackAble) {
                slot.amount+=amount;
                return true;
            }
        }

        if (!InventoryFull()) {
            InventoryItem newItem = new InventoryItem(item, amount);
            items.Add(newItem);
            return true;
        } else {
            DropItem(item, this.GetComponent<Entity>().GetPos());
            return true;
        }
    }

    public bool DropItem(Item item, Vector2Int pos) {
        int itemAmount = GetItemAmount(item);
        
        if (RemoveStack(item)) {
            Inventory.SpawnDroppedItem(droppedItemPrefab, item, itemAmount, pos);

            EventManager.InvokeEvent("PLAYER_TURN_FINISHED");

            return true;
        } else return false;
    }

    public virtual bool RemoveItem(Item item) {
        foreach (InventoryItem slot in items) {
            if (slot.item == item && item.stackAble) {
                slot.amount--;
                if (slot.amount <= 0) {
                    items.Remove(slot);
                }
                return true;
            } else if (slot.item == item) {
                items.Remove(slot);
                return true;
            }

        }

        return false;
    }

    public virtual bool RemoveStack(Item item) {
        foreach (InventoryItem slot in items) {
            if (slot.item == item) {
                items.Remove(slot);

                return true;
            }
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

    public void DebugItems() {
        foreach (InventoryItem item in Items) {
            if (item.item != null) {
                Debug.Log(item.item.itemName + " " + item.amount);
            }
        }
    }

    public static void SpawnDroppedItem(GameObject prefab, Item item, int amount, Vector2Int pos) {
        GameObject droppedItemObject = prefab;
        droppedItemObject.GetComponent<ItemPickup>().SetItem(item, amount);

        EntityManager.Instance.SpawnEntity(pos, droppedItemObject);
    }
}
