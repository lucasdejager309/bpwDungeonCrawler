using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Item[,] itemSlots = new Item[3,3];

    public bool AddItem(Item item) {
        if (InventoryFull(itemSlots)) {
            return false;
        } else {
            
            Vector2Int slot = FirstEmptySlot(itemSlots);
            itemSlots[slot.x, slot.y] = item;

            return true;
        }
    }

    public bool RemoveItem(Item item) {
        for(int x = 0; x < itemSlots.GetLength(0); x++) {
            for (int y = 0; y < itemSlots.GetLength(1); y++) {
                if (itemSlots[x,y] == item) {
                    itemSlots[x,y] = null;
                    return true;
                }
            }
        }

        return false;
    }

    private static Vector2Int FirstEmptySlot(Item[,] items) {
        for(int x = 0; x < items.GetLength(0); x++) {
            for (int y = 0; y < items.GetLength(1); y++) {
                if (items[x,y] != null) {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int();
    }

    private static bool InventoryFull(Item[,] items) {
        for(int x = 0; x < items.GetLength(0); x++) {
            for (int y = 0; y < items.GetLength(1); y++) {
                if (items[x,y] != null) {
                    return false;
                }
            }
        }

        return true;
    }
}
