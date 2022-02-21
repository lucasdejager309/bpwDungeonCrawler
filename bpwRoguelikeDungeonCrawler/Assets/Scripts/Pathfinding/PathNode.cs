using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PathNode : IHeapItem<PathNode> {
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private int heapIndex;

    public PathNode(Vector2Int pos) {
        this.pos = pos;
    }

    public PathNode cameFromNode = null;
    public Vector2Int pos;
    public int gCost = int.MaxValue;
    public int hCost;
    
    public int fCost {
        get {
            return gCost+hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(PathNode nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    public List<PathNode> GetNeighbouringNodes(List<Vector2Int> allowedPositions) {
        List<PathNode> neighbours = new List<PathNode>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                PathNode potentialNode = new PathNode(new Vector2Int(pos.x+x, pos.y+y));
                if (allowedPositions.Contains(potentialNode.pos)) {
                    neighbours.Add(potentialNode);
                }   
            }
        }

        return neighbours;
	}    

    public static int CalculateDistance(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.pos.x - b.pos.x);
        int yDistance = Mathf.Abs(a.pos.y - b.pos.y);

        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
}