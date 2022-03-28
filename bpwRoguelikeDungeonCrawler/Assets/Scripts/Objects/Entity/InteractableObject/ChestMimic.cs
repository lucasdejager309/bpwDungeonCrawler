using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMimic : InteractableObject
{
    public GameObject mimicPrefab;

    public override void Interact(GameObject interacter)
    {
        EnemyManager.Instance.SpawnEnemy(GetPos(), mimicPrefab);
        DeleteEntity();
    }

    public override void Die()
    {
        GetComponent<DropItems>().DropFromLootTable(GetPos());
        base.Die();
    }
}

