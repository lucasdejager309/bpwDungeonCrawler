using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseButtonAction : UIAction
{
    public override void DoAction()
    {
        GameManager.Instance.SetControlTo(ControlMode.INVENTORY);
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        Player player = GameManager.Instance.player.GetComponent<Player>();

        Item itemToUse = inventory.GetItem(inventory.pointerIndex);

        if (player.CheckIntelligence(itemToUse.requiredIntelligence) && player.CheckStrength(itemToUse.requiredStrength)) {
            if (itemToUse.equippable) {
                if (inventory.pointerIndex >= inventory.MAX_SLOTS) {
                    inventory.UnEquipItem(itemToUse.equipSlotID);
                } else {
                    inventory.EquipItem(itemToUse, itemToUse.equipSlotID);
                }     
            } else {
                itemToUse.Use();
            }
        } else {
            string textToLog = "";
            if (!player.CheckIntelligence(itemToUse.requiredIntelligence)) {
               textToLog = "You do not have the required intelligence to use " + itemToUse.itemName + "!";
            }
            if (!player.CheckStrength(itemToUse.requiredStrength)) {
                textToLog = "You do not have the required strength to use " + itemToUse.itemName + "!";
            }

            LogText.Instance.Log(textToLog);
        }
        base.DoAction();
    }
}
