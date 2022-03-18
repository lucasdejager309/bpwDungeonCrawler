using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseButtonAction : UIAction
{
    public override void DoAction()
    {
        GameManager.Instance.SetControlTo(GameManager.Controlling.INVENTORY);
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        Player player = GameManager.Instance.player.GetComponent<Player>();

        Item itemToUse = inventory.GetItem(inventory.pointerIndex);

        if (player.CheckInteligence(itemToUse.requiredInteligence) && player.CheckStrength(itemToUse.requiredStrength)) {
            if (itemToUse.equippable) {
                if (inventory.pointerIndex >= inventory.MAX_SLOTS) {
                    inventory.UnEquipItem(itemToUse.equipSlotID);
                } else {
                    inventory.EquipItem(itemToUse, itemToUse.equipSlotID);
                }     
            } else {
                itemToUse.Use();
            }
        }   
        base.DoAction();
    }
}
