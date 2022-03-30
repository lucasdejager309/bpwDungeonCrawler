using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyState {
    Down,
    Up,
    Hold
}


[System.Serializable]
public class InputKey {
    public KeyCode key;
    public KeyState state;
    public string action;
}

[CreateAssetMenu(fileName = "New InputScriptableObject", menuName = "InputScriptableObject")]
public class InputScriptableObject : ScriptableObject
{
    public InputKey[] inputs;

    public void GetInput() {
        foreach(InputKey input in inputs) {

            switch (input.state) {
                case KeyState.Down:

                    if (Input.GetKeyDown(input.key)) {
                        EventManager.InvokeEvent((string)input.action);
                    }
                    break;
                case KeyState.Up:

                    if (Input.GetKeyUp(input.key)) {
                        EventManager.InvokeEvent((string)input.action);
                    }
                    break;
                case KeyState.Hold:

                    if (Input.GetKey(input.key)) {
                        EventManager.InvokeEvent((string)input.action);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
