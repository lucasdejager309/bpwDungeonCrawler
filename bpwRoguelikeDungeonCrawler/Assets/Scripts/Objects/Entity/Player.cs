using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public float moveTime;
    public float timeBetweenMoves;
    public int moveDistance;
    public bool inputAllowed = true;

    [Header("player attributes")]
    int damage = 2;

    enum ActionType {
        MOVE,
        ATTACK,
        NOTHING
    }

    void Update() {
        Vector2Int input = GetInput();

        if (input != new Vector2Int(0,0) && GetActionType(input) != ActionType.NOTHING && inputAllowed) {
            inputAllowed = false;

            switch (GetActionType(input)) {
                case ActionType.MOVE:
                    
                    Task move = new Task(Move(GetInput(), moveDistance, true, moveTime, timeBetweenMoves));
                    move.Finished += delegate {
                        EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                    };

                    break;
                case ActionType.ATTACK:

                    Task attack = new Task(GetComponent<Attack>().DoAttack(input+GetPos(), damage, this));
                    attack.Finished += delegate {
                        EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                    };
                    
                    break;
                default:
                    break;
            }
        }
    }

    ActionType GetActionType(Vector2Int input)
    {
        Vector2Int pos = input + GetPos();
        ActionType actionToReturn = ActionType.NOTHING;
        Entity entity = EntityManager.Instance.EntityAtPos(pos);
        if (entity != null && entity.GetComponent<Enemy>() != null) {
            actionToReturn = ActionType.ATTACK;
        }
        else if (EntityManager.Instance.validPositions.Contains(pos)) {
            actionToReturn = ActionType.MOVE;
        }
        else actionToReturn = ActionType.NOTHING;
        return actionToReturn;
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
