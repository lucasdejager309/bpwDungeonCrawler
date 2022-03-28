using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryControlObject", menuName = "ControlObject/InventoryControlObject")]
public class InventoryControlObject : ControlObject
{
    public override void Interact()
    {
        UIManager.Instance.inventory.DoActionAtPointer();
    }

    public override void SetControlTo()
    {
        UIManager.Instance.inventory.TogglePanel(true);
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
    }

    public override void LoseControl()
    {
        UIManager.Instance.inventory.TogglePanel(false);
        UIManager.Instance.inventory.SetPointer(0);
    }

    public override void UpdateControl(Vector2Int input)
    {
        UIManager.Instance.inventory.UpdatePointer(input);
    }
} 