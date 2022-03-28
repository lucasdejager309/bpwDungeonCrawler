using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButtonAction : UIAction
{
    public override void DoAction()
    {
        GameManager.Instance.loadFromSave = false;
        GameManager.Instance.NewGame();
    }
}
