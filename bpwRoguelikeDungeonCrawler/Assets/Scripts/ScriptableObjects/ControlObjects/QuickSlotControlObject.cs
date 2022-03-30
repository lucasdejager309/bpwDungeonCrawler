using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuickSlotControlObject", menuName = "ControlObject/QuickSlotControlObject")]
public class QuickSlotControlObject : ControlObject
{
    public override void Interact() {
        //nothing
    }

    public override void SetControlTo() {
        UIManager.Instance.hotKeys.darkBackground.enabled = true;
        UIManager.Instance.hotKeys.promptText.enabled = true;
    }

    public override void LoseControl() {
        UIManager.Instance.hotKeys.darkBackground.enabled = false;
        UIManager.Instance.hotKeys.promptText.enabled = false;
    }

    public override void UpdateControl(Vector2Int input) {
        //nothing
    }
}
