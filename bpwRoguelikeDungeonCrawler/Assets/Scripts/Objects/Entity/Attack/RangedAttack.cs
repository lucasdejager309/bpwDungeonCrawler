using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Attack {
    public GameObject projectilePrefab;

    public override IEnumerator DoAttack(Vector2Int attackPos, int damage, Entity attacker) {
            Task attack = new Task(AttackAnimRanged(attackPos, attacker));

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

    IEnumerator AttackAnimRanged(Vector2Int attackPos, Entity attacker) {
        Vector3 startPos = attacker.transform.position + new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 endPos = (Vector3Int)attackPos + new Vector3(0.5f, 0.5f, 0.5f);
        
        GameObject projectile = Instantiate(projectilePrefab, startPos, Quaternion.identity);
        projectile.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y, -4);
        
        Vector2 dir = endPos - projectile.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float speed = Vector2.Distance(startPos, endPos)/animationSpeed;

        float elapsedTime = 0;
        while (elapsedTime < speed) {
            projectile.transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime/speed));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        GameObject.Destroy(projectile);
        

        yield break;
    }
}