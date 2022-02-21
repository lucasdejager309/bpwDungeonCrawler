using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    public LayerMask solidLayer;
    public bool entityIsSolid;

    void Start() {
        UpdatePosInDict();
    }

    protected virtual IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
            UpdatePosInDict(); //ugly fix

            //get actual position to move to (incase target position is unreachable)
            Vector2Int newPos = EntityMovement.GetNewPosition(transform, direction, distance);

            //move to actual position
            if (EntityMovement.IsValidMovePos(newPos)) {
                if (!smoothMove) {
                    transform.position = new Vector3(newPos.x, newPos.y, 0);
                } else {
                    Vector3 startPos = transform.position;
                    Task t = new Task(EntityMovement.SmoothMove(transform, newPos, moveTime/distance, waitBetweenMoves));
                    t.Finished += delegate (bool manual) {
                        transform.position = new Vector3(newPos.x, newPos.y, 0);
                    };
                    yield return new WaitForSeconds(moveTime+waitBetweenMoves);
                }
                UpdatePosInDict();
            }
    }

    public virtual IEnumerator DoAction() {
        yield break;
    }

    public bool TileInSight(Vector2 posToCheck) {
       Vector2 pos = new Vector2(transform.position.x+0.5f, transform.position.y+0.5f);
       RaycastHit2D hit = Physics2D.Raycast(pos, (posToCheck-pos), (posToCheck-pos).magnitude, solidLayer);
       if (hit.collider != null) {
           if (hit.collider.tag == "solidTileMap") {
               return false;
           } else return true;
       } else return true;
    }

    public virtual void DeleteEntity() {
        EntityManager.Instance.entityDict.Remove(this);
        // foreach(KeyValuePair<Entity, Vector2Int> entity in EntityManager.Instance.entityDict) {
        //     if (entity.Key == this) {
        //         EntityManager.Instance.entityDict.Remove(entity.Key);
        //         break;
        //     }
        // }

        GameObject.Destroy(gameObject);
    }

    public Vector2Int GetPos() {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public bool UpdatePosInDict() {
        foreach(KeyValuePair<Entity, Vector2Int> entity in EntityManager.Instance.entityDict) {
            if (entity.Key == this) {
                if (entity.Value != new Vector2Int((int)transform.position.x, (int)transform.position.y)) {
                    EntityManager.Instance.entityDict.Remove(entity.Key);
                    EntityManager.Instance.entityDict.Add(this, new Vector2Int((int)transform.position.x, (int)transform.position.y));

                    return true;
                } 

                return false;
            }
        }
        
        //if object not yet in dictionary;
        EntityManager.Instance.entityDict.Add(this, new Vector2Int((int)transform.position.x, (int)transform.position.y));
        return true;
    }
}
