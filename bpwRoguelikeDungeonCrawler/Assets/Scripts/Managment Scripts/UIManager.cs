using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UISlot {
    public GameObject gameObject;
    public string slotID;
}

public class UIManager : Singleton<UIManager>
{
    public GameObject inventoryUI;

    void Awake() {
        Instance = this;

        EventManager.AddListener("UI_WAIT", UpdateWait);
    }

    void UpdateWait() {
        Image wait = GameObject.FindGameObjectWithTag("UIWait").GetComponent<Image>();
        wait.enabled = !wait.enabled;
    }
}
