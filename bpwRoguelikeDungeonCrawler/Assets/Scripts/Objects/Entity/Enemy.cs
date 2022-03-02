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
                t = new Task(GetComponent<MeleeAttack>().DoAttack(target.GetComponent<Entity>().GetPos(), damage, this));
                t.Finished += delegate {
                    finished = true;
                };
                    
                break;

            case enemyState.RANGEDATTACK:
                t = new Task(GetComponent<RangedAttack>().DoAttack(target.GetComponent<Entity>().GetPos(), damage, this));
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
        if (distance <= attackRange && distance <= 1.5f && GetComponent<MeleeAttack>() != null) {
            return enemyState.MELEEATTACK;
        } else if (distance <= attackRange && GetComponent<RangedAttack>() != null) {
            return enemyState.RANGEDATTACK;
        } else {
            return enemyState.CHASE;
        }
    }

    public override void DeleteEntity()
    {
        EnemyManager.Instance.enemies.Remove(this.gameObject);

        base.DeleteEntity();
    }
}
