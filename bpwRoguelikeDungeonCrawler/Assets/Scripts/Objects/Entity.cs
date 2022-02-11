using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    void Start() {
        UpdatePosInDict();
    }

    public void Move(Vector2Int direction, int distance = 1) {
        Vector2Int newPos = new Vector2Int((int)transform.position.x + direction.x*distance, (int)transform.position.y + direction.y*distance);
        if (EntityManager.Instance.validPositions.Contains(newPos) && !EntityManager.Instance.entityPositions.ContainsKey(newPos)) {
            transform.position += new Vector3(direction.x*distance, direction.y*distance, 0);

            UpdatePosInDict();

        }
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
