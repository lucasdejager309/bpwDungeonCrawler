using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    public LayerMask solidLayer;

    void Start() {
        UpdatePosInDict();
    }

    protected virtual IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
            UpdatePosInDict(); //ugly fix

            //get actual position to move to (incase target position is unreachable)
            Vector2Int newPos = GetNewPosition(direction, distance);

            //move to actual position
            if (IsValidMovePos(newPos)) {
                if (!smoothMove) {
                    transform.position = new Vector3(newPos.x, newPos.y, 0);
                } else {
                    Vector3 startPos = transform.position;
                    Task t = new Task(SmoothMove(newPos, moveTime/distance, waitBetweenMoves));
                    t.Finished += delegate (bool manual) {
                        transform.position = new Vector3(newPos.x, newPos.y, 0);
                    };
                    yield return new WaitForSeconds(moveTime+waitBetweenMoves);
                }
                UpdatePosInDict();
            }
    }

    public Vector2Int GetNewPosition(Vector2Int direction, int distance) {
        Vector2Int targetPos = new Vector2Int((int)transform.position.x + direction.x*distance, (int)transform.position.y + direction.y*distance);
        if (!IsValidMovePos(targetPos)) {
            for (int i = distance; i > 0; i--) {
                Vector2Int newPos = new Vector2Int((int)transform.position.x + direction.x*i, (int)transform.position.y + direction.y*i);
                if (IsValidMovePos(newPos)) {
                    return newPos;
                }
            }
        }
        return targetPos;
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

    bool IsValidMovePos(Vector2Int newPos) {
        Debug.Log(EntityManager.Instance.validPositions.Contains(newPos) + " "  + EntityManager.Instance.validPositions.Count);
        if (EntityManager.Instance.validPositions.Contains(newPos) && !EntityManager.Instance.entityDict.ContainsKey(newPos)) {
            return true;
        } return false;
    }

    IEnumerator SmoothMove(Vector2Int newPos, float speed, float waitBetweenMoves = 0) {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(newPos.x, newPos.y, transform.position.z);

        float elapsedTime = 0;
        

        while (elapsedTime < speed) {
            transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime/speed));
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        yield return new WaitForSeconds(waitBetweenMoves);
    }

    bool UpdatePosInDict() {
        foreach(KeyValuePair<Vector2Int, GameObject> entity in EntityManager.Instance.entityDict) {
            if (entity.Value == this.gameObject) {
                if (entity.Key != new Vector2Int((int)transform.position.x, (int)transform.position.y)) {
                    EntityManager.Instance.entityDict.Remove(entity.Key);
                    EntityManager.Instance.entityDict.Add(new Vector2Int((int)transform.position.x, (int)transform.position.y), this.gameObject);

                    return true;
                } 

                return false;
            }
        }
        
        //if object not yet in dictionary;
        EntityManager.Instance.entityDict.Add(new Vector2Int((int)transform.position.x, (int)transform.position.y), this.gameObject);
        return true;
    }
}
