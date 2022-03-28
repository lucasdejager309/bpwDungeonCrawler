using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspectMenuControlObject", menuName = "ControlObject/InspectMenuControlObject")]
public class InspectMenuControlObject : ControlObject
{
    public override void Interact()
    {
        //nothing
    }

    public override void SetControlTo()
    {
        UIManager.Instance.inspectPanel.TogglePanel(true);
    }

    public override void LoseControl()
    {
        UIManager.Instance.inspectPanel.TogglePanel(false);
    }

    public override void UpdateControl(Vector2Int input)
    {
        
    }
}
 