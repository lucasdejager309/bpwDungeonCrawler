using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipSlot {
    public string slotID;
    public Item item;
}

class PlayerInventory : Inventory {
    
    public EquipSlot[] equipSlots;
    public int pointerIndex = 0;
    public bool inputAllowed = true;

    public bool UpdateInventoryPointer(Vector2Int input) {
        bool succeeded = false;
        if (input != new Vector2Int(0, 0) && inputAllowed) {
            //REGULAR INVENTORY SLOTS
            if (pointerIndex < MAX_SLOTS) {
                if (Mathf.Abs(input.x) == 1) {
                if (pointerIndex+input.x >= 0 && pointerIndex+input.x < MAX_SLOTS) {
                    pointerIndex+=input.x;
                    succeeded = true;
                } else succeeded = false;
                
                }
                if (Mathf.Abs(input.y) == 1) {
                    if (pointerIndex-input.y*ROW_SIZE >= 0 && pointerIndex-input.y*ROW_SIZE < MAX_SLOTS) {
                        pointerIndex-=input.y*ROW_SIZE;
                        succeeded = true;
                    } else if (pointerIndex < ROW_SIZE && input.y == 1) {
                        pointerIndex = MAX_SLOTS;
                        succeeded = true;
                    }
                }
            //EQUIP SLOTS
            } else {
                if (Mathf.Abs(input.x) == 1) {
                    if (pointerIndex+input.x >= MAX_SLOTS && pointerIndex+input.x < MAX_SLOTS+equipSlots.Length) {
                        pointerIndex+=input.x;
                    }
                }

                if (input.y == -1) {
                    pointerIndex = 0;
                }
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

    public bool EquipItem(Item item, string slotID) {
        if (RemoveItem(item)) {
            foreach (EquipSlot slot in equipSlots) {
                if (slot.slotID == slotID) {
                    slot.item = item;
                    EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
                    return true;
                }
            }

            return false;

        } else return false;
    }

    public override Item GetItem(int index) {
        if (index < Items.Count) {
            return Items[index].item;
        } else if (index >= MAX_SLOTS && index < MAX_SLOTS+equipSlots.Length) {
            return equipSlots[index-MAX_SLOTS].item;
        }

        return null;
    }
}