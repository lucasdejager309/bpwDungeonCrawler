using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public float animationSpeed;

    public int howManyTurns = 1;
    public int lastTurnDone;

    public virtual IEnumerator DoAttack(Vector2Int attackPos, int damage, Entity attacker) {
        lastTurnDone = GameManager.Instance.CurrentTurn;
        
        List<Entity> entities = EntityManager.Instance.EntitiesAtPos(attackPos);
        foreach(Entity entity in entities) {
            if (entity != null) {
                entity.TakeDamage(damage);
                EventManager.InvokeEvent("DAMAGE_HAPPENED");
            }
        }

        return null;
    }

    public bool AttackIsAllowed() {
        int currentTurn = GameManager.Instance.CurrentTurn;
        
        if (lastTurnDone == 0) {
            return true;
        }

        //Debug.Log(this.gameObject.name + " currentTurn: " + GameManager.Instance.CurrentTurn + " lastTurnDone: " + lastTurnDone + " lastTurnDone + howManyTurns: " + (lastTurnDone + howManyTurns));

        if ((lastTurnDone + howManyTurns) <= currentTurn) {
            return true;
        } else return false;
    }
}