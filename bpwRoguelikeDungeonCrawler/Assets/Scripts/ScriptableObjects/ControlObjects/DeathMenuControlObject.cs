using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathMenuControlObject", menuName = "ControlObject/DeathMenuControlObject")]
public class DeathMenuControlObject : ControlObject
{
    public override void Interact() {
        UIManager.Instance.deathMenu.DoActionAtPointer();
    }

    public override void SetControlTo() {
        UIManager.Instance.deathMenu.TogglePanel(true);
    }

    public override void LoseControl() {
        UIManager.Instance.deathMenu.TogglePanel(false);
        UIManager.Instance.deathMenu.SetPointer(0);
    }

    public override void UpdateControl(Vector2Int input) {
        UIManager.Instance.deathMenu.UpdatePointer(input);
    }
}
