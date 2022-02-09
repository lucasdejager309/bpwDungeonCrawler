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

public class InputManager : Singleton<InputManager>
{
    public InputKey[] inputs;

    void Awake() {
        Instance = this;
    }

    void Update() {
        
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