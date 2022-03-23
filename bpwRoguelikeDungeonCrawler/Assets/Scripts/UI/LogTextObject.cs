using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogTextObject : MonoBehaviour
{
    float displayTime;

    void Start() {
        displayTime = LogText.Instance.displayMessageFor;

        Invoke("Fade", displayTime/2);
    }

    void Fade() {
        Task t = new Task(UIManager.Instance.FadeText(GetComponent<Text>(), displayTime/2));
        t.Finished += delegate {
            LogText.Instance.RemoveLog(this.gameObject);
        };
    }
}
