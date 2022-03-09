using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public float timeBetweenMoves;
    public int moveDistance;
    public bool inputAllowed = true;

    [Header("player attributes")]
    public int baseDamage = 2;
    [SerializeField] private int strength = 11;
    public int Strength {
        get { return strength; }
    }
    [SerializeField] private int inteligence = 11;
    public int Inteligence {
        get { return inteligence; }
    }

    enum ActionType {
        MOVE,
        ATTACK,
        INTERACT,
        NOTHING
    }

    public bool CheckStrength(int value) {
        if (strength >= value) {
            return true;
        }
        return false;
    }

    public void SetStrength(int value) {
        strength = value;
        UIManager.Instance.UpdateStats();
    }

    public bool CheckInteligence(int value) {
        if (inteligence >= value) {
            return true;
        }
        return false;
    }

    public void SetInteligence(int value) {
        inteligence = value;
        UIManager.Instance.UpdateStats();
    }

    public override void TakeDamage(int damage)
    {
        ArmorItem armor = (ArmorItem)GetComponent<PlayerInventory>().GetItemBySlotID("ARMOR");
        if (armor != null) {
            damage -= armor.AbsorbDamage();
            if (damage < 0) damage = 0;
        }

        base.TakeDamage(damage);

        EventManager.InvokeEvent("UI_UPDATE_STATS");
    }

    public override void SetHealth(int newHealth)
    {
        base.SetHealth(newHealth);
        EventManager.InvokeEvent("UI_UPDATE_STATS");
    }

    public int CalculateDamage(IWeapon weapon) {
        int damage = 0;
        if (weapon != null) {
            damage = weapon.GetDamage();
        } else {
            damage = baseDamage;
        }
        
        damage += (int)Random.Range(0, Strength/6);

        return damage;
    }

    public void UpdatePlayer(Vector2Int input) {
        Task action = new Task();

        if (input != new Vector2Int(0,0) && GetActionType(input) != ActionType.NOTHING && inputAllowed) {
            inputAllowed = false;

            switch (GetActionType(input)) {
                case ActionType.MOVE:
                    
                    action = new Task(Move(input, moveDistance, true, 0.1f, timeBetweenMoves));
                    
                    break;
                case ActionType.ATTACK:
                    action = new Task(GetComponent<MeleeAttack>().DoAttack(input+GetPos(), CalculateDamage((IWeapon)GetComponent<PlayerInventory>().GetItemBySlotID("WEAPON")), this));
                    
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
        if (entity != null && entity.entityIsSolid) {
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

    public override void Start() {
        base.Start();
        EventManager.AddListener("OTHER_TURNS_FINISHED", AllowInput);
    }
    
    void AllowInput() {
        inputAllowed = true;
    }

    protected override IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
        StartCoroutine(base.Move(direction, distance, smoothMove, moveTime, waitBetweenMoves));
        
        yield return new WaitForSeconds(moveTime + timeBetweenMoves);
    }
}
