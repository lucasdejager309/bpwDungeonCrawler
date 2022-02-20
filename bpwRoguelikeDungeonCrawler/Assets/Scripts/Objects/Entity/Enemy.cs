using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    List<Vector2Int> path = new List<Vector2Int>();

    public override IEnumerator DoAction()
    {
        //path = Pathfinding.GetPath(new Vector2Int((int)transform.position.x, (int)transform.position.y), new Vector2Int(0,0), EntityManager.Instance.validPositions);
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
