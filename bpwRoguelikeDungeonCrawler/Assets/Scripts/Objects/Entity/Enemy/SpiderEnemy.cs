using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : Enemy
{
    [Header("Spider Business")]
    public GameObject prefabToMultiply;
    public float underlingHealthModifier = 0.5f;

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (Health > 0) {
            Multiply();
        }
    }

    bool Multiply() {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) { continue;}
                Vector2Int pos = target.GetComponent<Entity>().GetPos() + new Vector2Int(x, y);
                if (EntityManager.Instance.validPositions.Contains(pos) && !EntityManager.Instance.entityDict.ContainsValue(pos)) {
                    GameManager.Instance.DrawCross(pos, 1f, Color.red);
                    
                    GameObject spawnedSpider = EnemyManager.Instance.SpawnEnemy(pos, prefabToMultiply);
                    spawnedSpider.GetComponent<Enemy>().SetHealth((int)(Health*underlingHealthModifier));
                    spawnedSpider.GetComponent<Enemy>().SetMaxHealth((int)(Health*underlingHealthModifier));
                    spawnedSpider.GetComponent<DropItems>().privateTable = new List<Item>();
                    return true;
                }
            }
        }

        return false;
    }
}
