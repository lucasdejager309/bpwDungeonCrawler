using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    void Start() {
        EventManager.AddListener("PLAYER_UP", MoveUp);
        EventManager.AddListener("PLAYER_DOWN", MoveDown);
        EventManager.AddListener("PLAYER_LEFT", MoveLeft);
        EventManager.AddListener("PLAYER_RIGHT", MoveRight);
    }

    void MoveUp() { Move(new Vector2Int(0,1), 1, true, 0.1f); }
    void MoveDown() { Move(new Vector2Int(0,-1), 1, true, 0.1f); }
    void MoveLeft() { Move(new Vector2Int(-1,0), 1, true, 0.1f); }
    void MoveRight() { Move(new Vector2Int(1,0), 1, true, 0.1f); }
}
