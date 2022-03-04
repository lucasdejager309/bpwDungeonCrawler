using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public override IEnumerator DoAttack(Vector2Int attackPos, int damage, Entity attacker)
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

    public static IEnumerator AttackAnimMelee(Vector2Int attackPos, Entity attacker, float animationSpeed) {
        Vector3 startPos = attacker.transform.position;
        Vector3 difference = attacker.transform.position - (Vector3Int)attackPos;
        Vector3 midPos = attacker.transform.position - difference/2;
        
        float speed = (Vector2.Distance(startPos, midPos)/animationSpeed)*2;

        float elapsedTime = 0;
        while (elapsedTime < speed) {
            attacker.transform.position = Vector3.Lerp(startPos, midPos, (elapsedTime/speed));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        elapsedTime = 0;
        while (elapsedTime < speed) {
            attacker.transform.position = Vector3.Lerp(midPos, startPos, (elapsedTime/speed));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        attacker.transform.position = startPos;
    }
}
