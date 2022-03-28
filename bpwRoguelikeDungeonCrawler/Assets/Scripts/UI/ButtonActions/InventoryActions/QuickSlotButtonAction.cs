using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotButtonAction : UIAction
{
    public override void DoAction()
    {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        Item itemToQuickslot = inventory.GetItem(inventory.pointerIndex);

        Task t = new Task(UIManager.Instance.hotKeys.GetQuickSlotInput(itemToQuickslot));
        t.Finished += delegate {
            base.DoAction();
        };
    }

    
}
