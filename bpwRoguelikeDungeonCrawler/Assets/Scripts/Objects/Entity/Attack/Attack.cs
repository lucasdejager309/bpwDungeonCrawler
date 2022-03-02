using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public virtual IEnumerator DoAttack(Vector2Int attackPos, int damage, Entity attacker, float animationSpeed = 0.1f) {
        Entity entity = EntityManager.Instance.EntityAtPos(attackPos);
        if (entity != null) entity.TakeDamage(damage);
        return null;
    }
}