using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public float moveTime;
    public float timeBetweenMoves;
    public int moveDistance;
    public bool inputAllowed = true;

    void Update() {
        if (GetInput() != new Vector2Int(0,0) && inputAllowed) {
            inputAllowed = false;
            Task t = new Task(Move(GetInput(), moveDistance, true, moveTime, timeBetweenMoves));
            GetComponent<Animator>().SetBool("isWalking", true);
            t.Finished += delegate {
                EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                GetComponent<Animator>().SetBool("isWalking", false);
                EventManager.InvokeEvent("INTERACT");
            };
        }
    }

    void Start() {
        EventManager.AddListener("OTHERS_TURN_FINISHED", AllowInput);
    }
    
    void AllowInput() {
        inputAllowed = true;
    }

    protected override IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
        StartCoroutine(base.Move(direction, distance, smoothMove, moveTime, waitBetweenMoves));
        
        yield return new WaitForSeconds(moveTime + timeBetweenMoves);
    }

    //temp
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
