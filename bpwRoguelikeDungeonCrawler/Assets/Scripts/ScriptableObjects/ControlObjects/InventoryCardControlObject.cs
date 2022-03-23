using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryCardControlObject", menuName = "ControlObject/InventoryCardControlObject")]
public class InventoryCardControlObject : ControlObject
{
    public override void Interact()
    {
        UIManager.Instance.inventoryCard.DoActionAtPointer();
    }

    public override void SetControlTo()
    {
        UIInventory inventory = UIManager.Instance.inventory;
        UIPanel inventoryCard = UIManager.Instance.inventoryCard;

        inventory.UpdateCard();
        inventoryCard.TogglePanel(true);
        inventoryCard.SetPointer(0);
    }

    public override void LoseControl()
    {
        UIManager.Instance.inventoryCard.TogglePanel(false);
        UIManager.Instance.inventoryCard.SetPointer(0);
    }

    public override void UpdateControl(Vector2Int input)
    {
        UIManager.Instance.inventoryCard.UpdatePointer(input);
    }
} 
