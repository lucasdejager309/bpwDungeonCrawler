using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public List<Vector2Int> validPositions = new List<Vector2Int>();
    public List<Vector2Int> pathPositions = new List<Vector2Int>();
    public Vector2Int startPos;
    public Vector2Int endPos;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = -10; x <= 10; x++) {
            for (int y = -10; y <= 10; y++) {
                validPositions.Add(new Vector2Int(x, y));
            }
        }
        Pathfinding pathfinder = new Pathfinding();
        pathPositions = pathfinder.FindPath(startPos, endPos, validPositions);
    }

    void Update() {
        if (pathPositions != null) {
            for (int i = 0; i < pathPositions.Count; i++) {
                Debug.DrawLine(((Vector3Int)pathPositions[i]), ((Vector3Int)pathPositions[i+1]));
            }
        }
    }
}
