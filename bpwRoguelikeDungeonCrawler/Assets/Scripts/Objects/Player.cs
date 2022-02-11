using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    void Update() {
        Vector2Int movement = new Vector2Int((int)Input.GetAxis("Horizontal"), (int)Input.GetAxis("Vertical"));
        Debug.Log(movement);
        Move(movement, 1);
    }
}
