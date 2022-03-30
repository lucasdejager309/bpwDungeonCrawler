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
    [SerializeField] private int intelligence = 10;
    public int Intelligence {
        get { return intelligence; }
    }

    int startStrength;
    int startIntelligence;

    enum ActionType {
        MOVE,
        ATTACK,
        INTERACT,
        NOTHING
    }

    public override void Start() {
        base.Start();
        EventManager.AddListener("OTHER_TURNS_FINISHED", AllowInput);
        
        startStrength = strength;
        startIntelligence = intelligence;
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

    public bool CheckIntelligence(int value) {
        if (intelligence >= value) {
            return true;
        }
        return false;
    }

    public void SetIntelligence(int value) {
        intelligence = value;
        UIManager.Instance.UpdateStats();
    }

    public void ResetStats() {
        SetHealth(MaxHealth);
        SetStrength(startStrength);
        SetIntelligence(startIntelligence);
    }

    public override void TakeDamage(int damage)
    {
        ArmorItem armor = (ArmorItem)GetComponent<PlayerInventory>().GetItemBySlotID("ARMOR");
        if (armor != null) {
            damage -= armor.AbsorbDamage();
            if (damage < 0) damage = 0;
        }

        base.TakeDamage(damage);

        EventManager.InvokeEvent("UI_UPDATE_HEALTH");
    }

    public override void SetHealth(int newHealth)
    {
        base.SetHealth(newHealth);
        EventManager.InvokeEvent("UI_UPDATE_HEALTH");
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
            SetInputAllowed(false);

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
            GameManager.Instance.AddTurn();
        };
    }

    ActionType GetActionType(Vector2Int input)
    {
        Vector2Int pos = input + GetPos();
        
        ActionType actionToReturn = ActionType.NOTHING;

        List<Entity> entities = EntityManager.Instance.EntitiesAtPos(pos);

        bool containsEnemy = false;
        bool containsInteractable = false;
        foreach (Entity entity in entities) {
            if (entity != null && entity.isSolid) {
                if (entity.GetComponent<Enemy>() != null && GetComponent<Attack>().AttackIsAllowed()) {
                    containsEnemy = true;
                }
                if (entity.GetComponent<InteractableObject>() != null) {
                    containsInteractable = true;
                }
            }
        }

        if (containsEnemy) {
            actionToReturn = ActionType.ATTACK;
        } else if (containsInteractable) {
            actionToReturn=  ActionType.INTERACT;
        } else if (EntityManager.Instance.validPositions.Contains(pos)) {
            actionToReturn = ActionType.MOVE;
        }
        else actionToReturn = ActionType.NOTHING;
        
        
        return actionToReturn;
    }

    public void SetInputAllowed(bool state) {
        inputAllowed = state;
    }

    void AllowInput() {
        SetInputAllowed(true);
    }

    public override InspectInfo GetInfo()
    {
        InspectInfo info =  base.GetInfo();
        info.description = Health + "/" + MaxHealth + " HP\n\n" + info.description;
        return info;
    }

    public override void Die()
    {
        GameManager.Instance.SetControlTo(ControlMode.DEATH_MENU);
    }

    protected override IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
        StartCoroutine(base.Move(direction, distance, smoothMove, moveTime, waitBetweenMoves));
        
        yield return new WaitForSeconds(moveTime + timeBetweenMoves);
    }
}
