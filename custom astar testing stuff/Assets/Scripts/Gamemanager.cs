using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public Vector2Int gridSize;
    public List<Vector2Int> validPositions = new List<Vector2Int>();
    public List<PathNode> pathNodes = new List<PathNode>();
    public Vector2Int startPos;
    public Vector2Int endPos;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x <= gridSize.x; x++) {
            for (int y = 0; y <= gridSize.y; y++) {
                validPositions.Add(new Vector2Int(x, y));
            }
        }
        pathNodes = Pathfinding.FindPath(new PathNode(startPos), new PathNode(endPos), PathNode.Vector2IntListToNodes(validPositions));
        
        int index = 0;
        foreach(PathNode node in pathNodes) {
            Debug.Log(index + ", position: " + node.pos +  ", came from: " + node.cameFromNode);
            index++;
        }
    }

    void Update() {
        if (pathNodes != null) {
            for (int i = 0; i < pathNodes.Count-1; i++) {
                Debug.DrawLine(((Vector3Int)pathNodes[i].pos), ((Vector3Int)pathNodes[i+1].pos));
            }
        }
    }
}
