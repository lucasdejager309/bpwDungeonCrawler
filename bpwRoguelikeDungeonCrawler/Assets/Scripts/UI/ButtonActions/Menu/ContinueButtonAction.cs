using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButtonAction : UIAction {
    public override void DoAction()
    {
        GameManager.Instance.SetControlTo(GameManager.Controlling.PLAYER);
    }
}
