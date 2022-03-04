using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public enum enemyState {
        CHASE,
        MELEEATTACK,
        RANGEDATTACK,
        FLEE
    }

    List<PathNode> path = new List<PathNode>();
    public float sightRange;
    public float attackRange = 1f;
    public int moveSpeed = 1;
    public int damage = 1;

    public GameObject target;

    void Start() {
        target = GameManager.Instance.player;
    }

    public override IEnumerator DoAction()
    {
        Task t = new Task();
        bool finished = false;
        switch (GetState()) {
            case enemyState.CHASE:
                t = new Task(MoveToWards(target, moveSpeed, 0.2f));
                t.Finished += delegate {
                    finished = true;
                };
                break;

            case enemyState.MELEEATTACK:
                t = new Task(GetComponent<MeleeAttack>().DoAttack(EntityManager.Instance.PosOfEntity(target.GetComponent<Entity>()), damage, this));
                t.Finished += delegate {
                    finished = true;
                };
                    
                break;

            case enemyState.RANGEDATTACK:
                t = new Task(GetComponent<RangedAttack>().DoAttack(EntityManager.Instance.PosOfEntity(target.GetComponent<Entity>()), damage, this));
                t.Finished += delegate {
                    finished = true;
                };
                    
                break;

            default:
                break;      
        };

        while (true) {
            if (finished) {
                yield break;
            } else yield return null;
        }
    }

    public enemyState GetState() {
        float distance = GetDistance(target.GetComponent<Entity>().GetPos());
        if (distance <= attackRange) {
            if (distance <= 1.5f && GetComponent<MeleeAttack>() != null) {
                if (GetComponent<MeleeAttack>().AttackIsAllowed()) {
                    return enemyState.MELEEATTACK;
                }
            } else if (GetComponent<RangedAttack>() != null) {
                if (GetComponent<RangedAttack>().AttackIsAllowed() && GetComponent<RangedAttack>().HasAimOnTarget(target.GetComponent<Entity>())) {
                    return enemyState.RANGEDATTACK;
                }
            }
        }
        return enemyState.CHASE;
    }

    public override void DeleteEntity()
    {
        EnemyManager.Instance.enemies.Remove(this.gameObject);

        base.DeleteEntity();
    }
}