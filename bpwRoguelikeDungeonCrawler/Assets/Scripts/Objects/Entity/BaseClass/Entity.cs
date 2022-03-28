using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour, IDamagable
{
    public LayerMask solidLayer;
    
    [Header("entity attributes")]
    public string entityName;
    [TextArea(15,20)]
    public string description;
    public bool cantBeDestroyed = false;
    
    public bool isSolid;
    public bool isAttackable;
    
    [SerializeField]private int maxHealth;
    public int MaxHealth {
        get { return maxHealth; }
    }
    [SerializeField] private int health;
    public int Health {
        get { return health; }
    }

    public virtual void Start() {
        EntityManager.Instance.UpdatePos(this, GetPos());
        if (health == 0) {
            health = maxHealth;
        }
    }
    
    public virtual void TakeDamage(int damage) {
        health -= damage; 

        if (health <= 0) {
            Die();
        }
    }

    public virtual void SetHealth(int newHealth) {
        health = newHealth;
        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;
    }

    public virtual void SetMaxHealth(int newMaxHealth) {
        maxHealth = newMaxHealth;
        if (health > maxHealth) health = maxHealth;
    }
    
    public virtual void Die() {
        DropItems dropItems = GetComponent<DropItems>();
        if (dropItems != null) {
            dropItems.DropFromLootTable(GetPos());
        }
        DeleteEntity();
    }

    
    protected virtual IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
            //get actual position to move to (incase target position is unreachable)
            
            
            Vector2Int newPos = EntityMovement.GetNewPosition(transform, direction, distance);

            //move to actual position
            if (EntityMovement.IsValidMovePos(newPos)) {
                if (!smoothMove) {
                    SetPos(newPos);

                } else {
                    Vector3 startPos = transform.position;
                    Task t = new Task(SmoothMove(transform, newPos, moveTime/distance, waitBetweenMoves));
                    t.Finished += delegate (bool manual) {
                        SetPos(newPos);
                        
                        EntityManager.Instance.UpdatePos(this, newPos);

                    };
                    yield return new WaitForSeconds(moveTime+waitBetweenMoves);
                }
            } else yield break;
    }

    public IEnumerator MoveToWards(GameObject target, int tilesPerMove, float moveSpeed) {
        Vector2Int targetPos = target.GetComponent<Entity>().GetPos();
        
        List<PathNode> path = Pathfinding.FindPath(new PathNode(GetPos()), new PathNode(targetPos), EntityManager.Instance.validPositions);
        if (path != null) {
            int i = 0;
            bool doingMove = false;
            while(i < tilesPerMove && i < path.Count) {
                if (!doingMove) {
                    doingMove = true;
                    Vector2 dir = GetDirToNeighbour(path[i].pos);
                    Task t = new Task(Move(new Vector2Int((int)dir.x, (int)dir.y), 1, true, moveSpeed));
                    t.Finished += delegate {
                        i++;
                        doingMove = false;
                    };
                }

                yield return null;
            }
        } else yield return null;
    }

    public IEnumerator SmoothMove(Transform originalPos, Vector2Int newPos, float speed, float waitBetweenMoves = 0) {
        Vector3 startPos = originalPos.position;
        Vector3 endPos = new Vector3(newPos.x, newPos.y, originalPos.position.z);

        float elapsedTime = 0;

        if (GetComponent<Animator>() != null) GetComponent<Animator>().SetBool("isWalking", true);        

        while (elapsedTime < speed) {
            originalPos.position = Vector3.Lerp(startPos, endPos, (elapsedTime/speed));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        if (GetComponent<Animator>() != null) GetComponent<Animator>().SetBool("isWalking", false);

        yield return new WaitForSeconds(waitBetweenMoves);
    }

    //IM SURE THERE IS A BETTER WAY TO DO THIS HOLY SHIT
    private Vector2Int GetDirToNeighbour(Vector2Int pos) {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) { continue;}

                if (GetPos()+new Vector2Int(x,y) == pos) {
                    return new Vector2Int(x,y);
                }
            }
        }
        return new Vector2Int(0,0);
    }

    public bool TileInSight(Vector2Int posToCheck) {
        Vector2 selfPos = new Vector2(GetPos().x+0.5f, GetPos().y+0.5f);

        bool inSight = false;

        foreach(Vector2Int checkPos in EntityManager.GetNeighbours(posToCheck, false, true)) {
            Vector2 posFloat = new Vector2(checkPos.x+0.5f, checkPos.y+0.5f);
            if (EntityManager.Instance.validPositions.Contains(checkPos)) {
                RaycastHit2D hit = Physics2D.Raycast(selfPos, (posFloat-selfPos), (posFloat-selfPos).magnitude, solidLayer);
                if (hit.collider != null) {
                    if (hit.collider.tag == "solidTileMap") {
                        continue;
                    } else inSight = true;
                } else inSight = true;
            }
        }

        return inSight;
    }

    public virtual IEnumerator DoAction() {
        yield break;
    }

    public virtual void DeleteEntity() {
        if (!cantBeDestroyed) {
            GameObject.Destroy(gameObject);
            EntityManager.Instance.entityDict.Remove(this);
        }
    } 

    public float GetDistance(Vector2Int pos) {
        return Vector2.Distance(GetPos(), pos);
    }

    public Vector2Int GetPos() {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public void SetPos(Vector2Int newPos) {
        EntityManager.Instance.UpdatePos(this, newPos);
        transform.position = new Vector3(newPos.x, newPos.y, 0);
    }

    public virtual InspectInfo GetInfo() {
        InspectInfo info = new InspectInfo();
        info.sprite = GetComponent<SpriteRenderer>().sprite;
        info.name = entityName;
        info.description = description;

        return info;
    }
}
