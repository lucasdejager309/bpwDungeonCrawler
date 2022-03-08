using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAction : MonoBehaviour
{
    public virtual void DoAction() {
        UIManager.Instance.inventory.UpdateInventory();
    }
}
