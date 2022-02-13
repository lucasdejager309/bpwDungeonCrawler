using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    bool allowedToMove = true;

    void Start() {
        UpdatePosInDict();
    }

    public bool Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveSpeed = 0.2f, float waitBetweenMoves = 0) {
        if (allowedToMove) {
            UpdatePosInDict(); //ugly fix
        
            //get actual position to move to (incase target position is unreachable)
            Vector2Int newPos = GetNewPosition(direction, distance);

            //move to actual position
            if (IsValidMovePos(newPos)) {
                if (!smoothMove) {
                    transform.position = new Vector3(newPos.x, newPos.y, 0);
                } else {
                    allowedToMove = false;
                    Vector3 startPos = transform.position;
                    Task t = new Task(SmoothMove(newPos, moveSpeed/distance, waitBetweenMoves));
                    t.Finished += delegate (bool manual) {
                        allowedToMove = true;
                        transform.position = new Vector3(newPos.x, newPos.y, 0);
                    };
                }
                UpdatePosInDict();
                return true;
            }
        }
        return false;
    }

    Vector2Int GetNewPosition(Vector2Int direction, int distance) {
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

    bool IsValidMovePos(Vector2Int newPos) {
        if (EntityManager.Instance.validPositions.Contains(newPos) && !EntityManager.Instance.entityPositions.ContainsKey(newPos)) {
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
        foreach(KeyValuePair<Vector2Int, GameObject> entity in EntityManager.Instance.entityPositions) {
            if (entity.Value == this.gameObject) {
                if (entity.Key != new Vector2Int((int)transform.position.x, (int)transform.position.y)) {
                    EntityManager.Instance.entityPositions.Remove(entity.Key);
                    EntityManager.Instance.entityPositions.Add(new Vector2Int((int)transform.position.x, (int)transform.position.y), this.gameObject);

                    return true;
                } 

                return false;
            }
        }
        
        //if object not yet in dictionary;
        EntityManager.Instance.entityPositions.Add(new Vector2Int((int)transform.position.x, (int)transform.position.y), this.gameObject);
        return true;
    }
}
