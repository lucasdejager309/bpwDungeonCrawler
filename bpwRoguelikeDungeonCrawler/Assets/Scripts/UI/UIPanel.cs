using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public enum ScrollDirection {
        HORIZONTAL,
        VERTICAL
    }

    public ScrollDirection scrollDirection = ScrollDirection.HORIZONTAL;
    public bool activeOnLoad;
    public GameObject[] selectablePanels;
    public GameObject pointer;
    private int pointerIndex;
    public int PointerIndex {
        get {
            return pointerIndex;
        }
    }

    void Start() {
        if (!activeOnLoad) {
            TogglePanel(false);
        }
    }

    public virtual void UpdatePointer(Vector2Int input) {
        switch (scrollDirection) {
            case ScrollDirection.HORIZONTAL:
                if (Mathf.Abs(input.x) == 1) {
                    if (pointerIndex+input.x < selectablePanels.Length && pointerIndex+input.x >= 0) {
                        SetPointer(pointerIndex+input.x);
                    }
                }
                break;
            case ScrollDirection.VERTICAL:
                if (Mathf.Abs(input.y) == 1) {
                    if (pointerIndex-input.y < selectablePanels.Length && pointerIndex-input.y >= 0) {
                        SetPointer(pointerIndex-input.y);
                    }
                }
                break;
            default:
                break;
        }
        
    }

    public virtual void TogglePanel(bool state) {
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
