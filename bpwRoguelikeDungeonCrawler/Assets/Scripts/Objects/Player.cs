using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public float moveTime;
    public float timeBetweenMoves;

    void Update() {
        Move(GetInput(), 1, true, moveTime, timeBetweenMoves);
    }

    Vector2Int GetInput() {
        Vector2Int input = new Vector2Int();

        if (Input.GetKey(KeyCode.W)) {
            input.y = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            input.y = -1;
        }
        if (Input.GetKey(KeyCode.A)) {
            input.x = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            input.x = 1;
        }

        return input;
    }
}
