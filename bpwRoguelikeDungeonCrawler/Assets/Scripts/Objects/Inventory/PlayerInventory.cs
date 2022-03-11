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

    public bool SetInventoryPointer(int index) {
        if (index < MAX_SLOTS+equipSlots.Length) {
            pointerIndex = index;
            return true;
        } else return false;
    }

    public override bool AddItem(Item item, int amount = 1)
    {
        bool succeeded = base.AddItem(item, amount);
        return succeeded;
    }

    public override bool RemoveItem(Item item)
    {
        bool succeeded = base.RemoveItem(item);
        return succeeded;
    }

    public bool EquipItem(Item item, string slotID) {
        if (GetComponent<Player>().CheckStrength(item.requiredStrength)) {
            if (RemoveItem(item)) {
                foreach (EquipSlot slot in equipSlots) {
                    if (slot.slotID == slotID) {
                        
                        if (slot.item != null) {
                            UnEquipItem(slot.slotID);
                        }
                        
                        slot.item = item;
                        
                        EventManager.InvokeEvent("PLAYER_TURN_FINISHED");

                        return true;
                    }
                }

                return false;

            } else return false;
        }  else return false;
    }
    
    public bool UnEquipItem(string slotID) {
        foreach(EquipSlot slot in equipSlots) {
            if (slot.slotID == slotID) {
                Item unequipedItem = slot.item;
                slot.item = null;
                AddItem(unequipedItem);
                
                EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                
                return true;
            }
        }

        return false;
    }

    public override Item GetItem(int index) {
        if (index < Items.Count) {
            return Items[index].item;
        } else if (index >= MAX_SLOTS && index < MAX_SLOTS+equipSlots.Length) {
            return equipSlots[index-MAX_SLOTS].item;
        }

        return null;
    }

    public Item GetItemBySlotID(string ID) {
        foreach (EquipSlot slot in equipSlots) {
            if (slot.slotID == ID) {
                return slot.item;
            }
        }

        return null;
    }
}