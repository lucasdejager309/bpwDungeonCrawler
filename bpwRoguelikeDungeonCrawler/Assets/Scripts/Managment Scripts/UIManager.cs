using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    void Awake() {
        Instance = this;

        EventManager.AddListener("UI_WAIT", UpdateWait);
    }

    void UpdateWait() {
        Image wait = GameObject.FindGameObjectWithTag("UIWait").GetComponent<Image>();
        wait.enabled = !wait.enabled;
    }
}
