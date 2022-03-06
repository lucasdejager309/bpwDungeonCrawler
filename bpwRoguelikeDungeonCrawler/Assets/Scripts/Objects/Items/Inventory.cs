using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary <Item, int> items = new Dictionary<Item, int>();
    public int maxSlots;

    public bool AddItem(Item item) {
        if (items.ContainsKey(item)) {
            items[item]++;
            return true;
        } else if (!InventoryFull()) {
            items.Add(item, 1);
            return true;
        } else return false;
    }

    public bool RemoveItem(Item item) {
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

    private bool InventoryFull() {
        if (items.Count >= maxSlots) {
            return true;
        } else return false;
    }

    //temp
    public void DebugLogContent() {
        foreach(var kv in items) {
            Debug.Log(kv.Key + " " + kv.Value);
        }
    }
}
