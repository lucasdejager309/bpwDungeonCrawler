using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public enum enemyState {
        CHASE,
        ATTACK,
        FLEE
    }
    public enemyState state = enemyState.CHASE; 

    public List<PathNode> path = new List<PathNode>();
    public float sightRange;

    public GameObject target;

    void Start() {
        target = GameManager.Instance.player;
    }

    public override IEnumerator DoAction()
    {
        switch (state) {
            case(enemyState.CHASE):
                //do chase stuff
                bool finished = false;
                Task t = new Task(MoveToWards(target, 1, 0.2f));
                t.Finished += delegate {
                    finished = true;
                };

                while (true) {
                    if (finished) {
                        yield break;
                    } else yield return null;
                }

            default:
                break;      
        };
    }

    public override void DeleteEntity()
    {
        foreach(GameObject enemy in EnemyManager.Instance.enemies) {
            if (enemy == this.gameObject) {
                EnemyManager.Instance.enemies.Remove(enemy);
                break;
            }
        }
        base.DeleteEntity();
    }
}
