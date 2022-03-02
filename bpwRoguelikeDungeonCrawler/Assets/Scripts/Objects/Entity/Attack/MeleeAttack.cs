using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public override IEnumerator DoAttack(Vector2Int attackPos, int damage, Entity attacker, float animationSpeed = 0.1F)
    {
        Task attack = new Task(AttackAnimMelee(attackPos, attacker, animationSpeed));
                
        bool finished  = false;
        attack.Finished += delegate {
            base.DoAttack(attackPos, damage, attacker);
            finished = true;
        };

        while (true) {
            if (finished) {
                yield break;
            } else yield return null;
        }
    }

    IEnumerator AttackAnimMelee(Vector2Int attackPos, Entity attacker, float animationSpeed) {
        Vector3 startPos = attacker.transform.position;
        Vector3 difference = attacker.transform.position - (Vector3Int)attackPos;
        Vector3 midPos = attacker.transform.position - difference/2;
        
        float elapsedTime = 0;
        while (elapsedTime < animationSpeed) {
            attacker.transform.position = Vector3.Lerp(startPos, midPos, (elapsedTime/animationSpeed));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        elapsedTime = 0;
        while (elapsedTime < animationSpeed) {
            attacker.transform.position = Vector3.Lerp(midPos, startPos, (elapsedTime/animationSpeed));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        attacker.transform.position = startPos;
    }
}
