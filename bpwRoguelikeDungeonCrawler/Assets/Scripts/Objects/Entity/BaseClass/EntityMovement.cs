using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement {
    public static Vector2Int GetNewPosition(Transform originalPos, Vector2Int direction, int distance) {
        Vector2Int targetPos = new Vector2Int((int)originalPos.position.x + direction.x*distance, (int)originalPos.position.y + direction.y*distance);
        if (!IsValidMovePos(targetPos)) {
            for (int i = distance; i > 0; i--) {
                Vector2Int newPos = new Vector2Int((int)originalPos.position.x + direction.x*i, (int)originalPos.position.y + direction.y*i);
                if (IsValidMovePos(newPos)) {
                    return newPos;
                }
            }
        }
        return targetPos;
    }

    public static bool IsValidMovePos(Vector2Int newPos) {
        bool entityAtNewPosIsSolid = true;
        foreach(KeyValuePair<Entity, Vector2Int> entry in EntityManager.Instance.entityDict) {
            if(entry.Value == newPos) {
                entityAtNewPosIsSolid = entry.Key.entityIsSolid;
            }
        }
        
        if (EntityManager.Instance.validPositions.Contains(newPos) && !EntityManager.Instance.entityDict.ContainsValue(newPos) || !entityAtNewPosIsSolid) {
            return true;
        } return false;
    }
}