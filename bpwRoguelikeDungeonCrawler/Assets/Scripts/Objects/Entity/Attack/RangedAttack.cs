using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Attack {
    public GameObject projectilePrefab;

    public override IEnumerator DoAttack(Vector2Int attackPos, int damage, Entity attacker, int attackRange = 1, GameObject impactEffect = null) {
            Task attack = new Task(AttackAnimRanged(attackPos, attacker));

            bool finished  = false;
            attack.Finished += delegate {
                int range = attackRange-1;
                for (int x = -range; x <= range; x++) {
                    for (int y = -range; y <= range; y++) {
                        Vector2Int pos = attackPos + new Vector2Int(x, y);
                        base.DoAttack(pos, damage, attacker, attackRange, impactEffect);
                        GameManager.Instance.DrawCross(pos, 1f, Color.red);
                    }
                }
                
                finished = true;
            };

            while (true) {
                if (finished) {
                    yield break;
                } else yield return null;
            }
    }

    public bool HasAimOnTarget(Vector2 pos) {
        Vector2 selfPos = new Vector2(GetComponent<Entity>().GetPos().x+0.5f, GetComponent<Entity>().GetPos().y+0.5f);
        Vector2 posFloat = new Vector2(pos.x+0.5f, pos.y+0.5f);
        
        RaycastHit2D hit = Physics2D.Raycast(selfPos, (posFloat-selfPos), (posFloat-selfPos).magnitude, GetComponent<Entity>().solidLayer);
        if (hit.collider != null) {
            if (hit.collider.tag == "solidTileMap") {
                return false;
            } else return true;
        } else return true;
    }

    IEnumerator AttackAnimRanged(Vector2Int attackPos, Entity attacker) {
        Vector3 startPos = (Vector3Int)attacker.GetPos() + new Vector3(0.5f, 0.5f, 0.5f);
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
