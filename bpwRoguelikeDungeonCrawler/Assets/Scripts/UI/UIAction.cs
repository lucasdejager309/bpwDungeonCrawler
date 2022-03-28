using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAction : MonoBehaviour
{
    public virtual void DoAction() {
        EventManager.InvokeEvent("UI_UPDATE_INVENTORY");
    }
}
