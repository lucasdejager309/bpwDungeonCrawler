using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public GameObject[] selectablePanels;
    public GameObject pointer;
    private int pointerIndex;
    public int PointerIndex {
        get {
            return pointerIndex;
        }
    }

    public virtual void UpdatePointer(Vector2Int input) {
        if (Mathf.Abs(input.x) == 1) {
            if (pointerIndex+input.x < selectablePanels.Length && pointerIndex+input.x >= 0) {
                SetPointer(pointerIndex+input.x);
            }
        }
    }

    public void TogglePanel(bool state) {
        this.gameObject.SetActive(state);
    }   

    public virtual void SetPointer(int newIndex) {
        pointerIndex = newIndex;
        pointer.transform.position = selectablePanels[pointerIndex].transform.position;
    }    

    public void DoActionAtPointer() {
        selectablePanels[pointerIndex].GetComponent<UIAction>().DoAction();
    }
}
