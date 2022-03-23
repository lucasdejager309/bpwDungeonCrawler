using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotAction : UIAction
{
    public override void DoAction() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        if (inventory.GetItem(inventory.pointerIndex) != null) {
            GameManager.Instance.SetControlTo(ControlMode.INVENTORYCARD);
        }
    }
}
