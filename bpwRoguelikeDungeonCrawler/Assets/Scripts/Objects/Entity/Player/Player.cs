using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public float timeBetweenMoves;
    public int moveDistance;
    public bool inputAllowed = true;

    [Header("player attributes")]
    int damage = 2;

    enum ActionType {
        MOVE,
        ATTACK,
        INTERACT,
        NOTHING
    }

    void Update() {
        Vector2Int input = GetInput();

        Task action = new Task();

        if (input != new Vector2Int(0,0) && GetActionType(input) != ActionType.NOTHING && inputAllowed) {
            inputAllowed = false;

            switch (GetActionType(input)) {
                case ActionType.MOVE:
                    
                    action = new Task(Move(GetInput(), moveDistance, true, 0.1f, timeBetweenMoves));
                    
                    break;
                case ActionType.ATTACK:
                    action = new Task(GetComponent<Attack>().DoAttack(input+GetPos(), damage, this));
                    
                    break;                
                case ActionType.INTERACT:
                    
                    action = new Task(GetComponent<Interact>().DoInteract(input+GetPos()));

                    break;
                default:
                    break;
            }
        }

        action.Finished += delegate {
            EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
            EventManager.InvokeEvent("ADD_TURN");
        };
    }

    ActionType GetActionType(Vector2Int input)
    {
        Vector2Int pos = input + GetPos();
        ActionType actionToReturn = ActionType.NOTHING;
        Entity entity = EntityManager.Instance.EntityAtPos(pos);
        if (entity != null) {
            if (entity.GetComponent<Enemy>() != null && GetComponent<Attack>().AttackIsAllowed()) {
                actionToReturn = ActionType.ATTACK;
            } else if (entity.GetComponent<InteractableObject>() != null) {
                actionToReturn = ActionType.INTERACT;
            }
        }
        else if (EntityManager.Instance.validPositions.Contains(pos)) {
            actionToReturn = ActionType.MOVE;
        }
        else actionToReturn = ActionType.NOTHING;
        return actionToReturn;
    }

    void Start() {
        EventManager.AddListener("OTHER_TURNS_FINISHED", AllowInput);
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
