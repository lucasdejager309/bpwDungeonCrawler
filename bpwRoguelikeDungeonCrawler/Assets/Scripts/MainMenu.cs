using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public UIPanel buttons;


    void Update() {
        buttons.UpdatePointer(GetInput());

        if (Input.GetKeyDown(KeyCode.Space)) {
            ClickButton();
        }
    }

    void ClickButton() {
        buttons.DoActionAtPointer();
    }

    Vector2Int GetInput() {
        Vector2Int input = new Vector2Int();

        if (Input.GetKeyDown(KeyCode.W)) {
            input.y = 1;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            input.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            input.x = -1;
        }
        if (Input.GetKeyDown
         (KeyCode.D)) {
            input.x = 1;
        }

        return input;
    }
}
