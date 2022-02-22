using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public List<PathNode> path = new List<PathNode>();

    public override IEnumerator DoAction()
    {
        yield return new WaitForSeconds(0.0f);
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
