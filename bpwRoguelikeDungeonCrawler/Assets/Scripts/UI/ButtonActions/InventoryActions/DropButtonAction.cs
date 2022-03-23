using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropButtonAction : UIAction
{
    public override void DoAction()
    {
        GameManager.Instance.SetControlTo(ControlMode.INVENTORY);
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        
        Item itemToDrop = inventory.GetItem(inventory.pointerIndex);

        if (inventory.pointerIndex >= inventory.MAX_SLOTS) {
            inventory.UnEquipItem(itemToDrop.equipSlotID);
            inventory.DropItem(itemToDrop, GameManager.Instance.player.GetComponent<Player>().GetPos());
        } else {
            inventory.DropItem(itemToDrop, GameManager.Instance.player.GetComponent<Player>().GetPos());
        }
        
        base.DoAction();
    }
}
