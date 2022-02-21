using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PathNode {
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

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

    public List<PathNode> GetNeighbouringNodes(List<PathNode> allowedNodes) {
        List<PathNode> neighbours = new List<PathNode>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                PathNode potentialNode = new PathNode(new Vector2Int(pos.x+x, pos.y+y));
                if (potentialNode.PosIsIn(allowedNodes)) {
                    neighbours.Add(potentialNode);
                }   
            }
        }

        return neighbours;
	}

    private bool PosIsIn(List<PathNode> allowedNodes) {
        foreach(PathNode node in allowedNodes) {
            if (node.pos == this.pos) {
                return true;
            }
        }

        return false;
    }

    public static int CalculateDistance(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.pos.x - b.pos.x);
        int yDistance = Mathf.Abs(a.pos.y - b.pos.y);

        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    public static PathNode GetLowestFcostNode(List<PathNode> nodes) {
        PathNode lowestFCostNode = nodes[0];
        foreach (PathNode node in nodes) {
            if (node.fCost < lowestFCostNode.fCost) {
                lowestFCostNode = node;
            }
        }

        return lowestFCostNode;
    }
    public static PathNode Vector2IntToNode(Vector2Int vector) {
        return new PathNode(vector);
    }

    public static List<PathNode> Vector2IntListToNodes(List<Vector2Int> vectors) {
        List<PathNode> nodes = new List<PathNode>();
        foreach(Vector2Int vector in vectors) {
            nodes.Add(new PathNode(vector));
        }
        return nodes;
    }
}