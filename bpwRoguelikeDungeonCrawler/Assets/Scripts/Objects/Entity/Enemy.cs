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
        foreach(KeyValuePair<Vector2Int, GameObject> enemy in EnemyManager.Instance.enemies) {
            if (enemy.Value == this.gameObject) {
                EnemyManager.Instance.enemies.Remove(enemy.Key);
                break;
            }
        }
        base.DeleteEntity();
    }
}
