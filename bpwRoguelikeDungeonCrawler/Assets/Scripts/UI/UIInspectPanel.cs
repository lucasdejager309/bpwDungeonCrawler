using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInspectPanel : UIPanel
{
    public GameObject spriteDisplay;
    public Text nameDisplay;
    public Text descriptionDisplay;

    void Start() {
        TogglePanel(false);
    }

    public void UpdateInfo(InspectInfo info) {
        spriteDisplay.GetComponent<Image>().sprite = info.sprite;
        nameDisplay.text = info.name;
        descriptionDisplay.text = info.description;
    }
}
