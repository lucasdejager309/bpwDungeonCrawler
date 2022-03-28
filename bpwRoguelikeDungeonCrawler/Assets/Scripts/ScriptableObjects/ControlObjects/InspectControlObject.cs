using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InspectControlObject", menuName = "ControlObject/InspectControlObject")]
public class InspectControlObject : AimPointerControlObject
{
    public override void DoActionAtPointer()
    {
        InspectInfo info = InspectInfo.GetInfo(UIManager.Instance.aimpointer.GetPos());
        UIManager.Instance.inspectPanel.UpdateInfo(info);
        GameManager.Instance.SetControlTo(ControlMode.INSPECT_MENU);
    }

    public override void SetControlTo()
    {
        base.SetControlTo();
        UIManager.Instance.aimpointer.IgnoreWalls(true);
    }
}
