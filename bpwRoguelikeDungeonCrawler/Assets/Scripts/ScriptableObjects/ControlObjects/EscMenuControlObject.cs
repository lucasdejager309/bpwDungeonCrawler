using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EscMenuControlObject", menuName = "ControlObject/EscMenuControlObject")]
public class EscMenuControlObject : ControlObject
{
    public override void Interact()
    {
        UIManager.Instance.escMenu.DoActionAtPointer();
    }

    public override void SetControlTo()
    {
        UIManager.Instance.escMenu.TogglePanel(true);
    }

    public override void LoseControl()
    {
        UIManager.Instance.escMenu.TogglePanel(false);
        UIManager.Instance.escMenu.SetPointer(0);
    }

    public override void UpdateControl(Vector2Int input)
    {
        UIManager.Instance.escMenu.UpdatePointer(input);
    }
}
 