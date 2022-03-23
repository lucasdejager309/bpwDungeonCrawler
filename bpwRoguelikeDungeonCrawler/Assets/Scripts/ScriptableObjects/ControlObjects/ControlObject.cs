using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode {
    PLAYER,
    INVENTORY,
    INVENTORYCARD,
    AIM_POINTER,
    ESC_MENU
}  

public class ControlObject : ScriptableObject {
    public ControlMode mode;
    
    public virtual void Interact() {

    }

    public virtual void SetControlTo() {

    }

    public virtual void LoseControl() {
        
    }

    public virtual void UpdateControl(Vector2Int input) {

    }
}