using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PlayerInventory : Inventory {
    
    public int pointerIndex = 0;
    public bool inputAllowed = true;

    public bool UpdateInventoryPointer(Vector2Int input) {
        bool succeeded = false;
        if (input != new Vector2Int(0, 0) && inputAllowed) {
            if (Mathf.Abs(input.x) == 1) {
                if (pointerIndex+input.x >= 0 && pointerIndex+input.x < MAX_SLOTS) {
                    pointerIndex+=input.x;
                    succeeded = true;
                } else succeeded = false;
                
            }
            if (Mathf.Abs(input.y) == 1) {
                if (pointerIndex-input.y*3 >= 0 && pointerIndex-input.y*3 < MAX_SLOTS) {
                    pointerIndex-=input.y*3;
                    succeeded = true;
                } else succeeded = false;
            }
        }
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY_POINTER");
        return succeeded;
    }

    public bool SetInventoryPointer(int index) {
        if (index < MAX_SLOTS) {
            pointerIndex = index;
            return true;
        } else return false;
    }

    public override bool AddItem(Item item)
    {
        bool succeeded = base.AddItem(item);
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
        return succeeded;
    }

    public override bool RemoveItem(Item item)
    {
        bool succeeded = base.RemoveItem(item);
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
        return succeeded;
    }

}