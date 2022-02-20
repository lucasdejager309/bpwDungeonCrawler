using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node {
    public Node(Vector2Int pos, List<Vector2Int> validPositions) {
        this.pos = pos;

        if (validPositions.Contains(pos)) {
            validPos = true;
        } else validPos = false;
    }

    public bool validPos;
    public Vector2Int pos;
    public int gCost;
    public int hCost;
    public int fCost {
        get {
            return gCost+hCost;
        }
    }

    public Node parent;
}

public class Pathfinding {

    List<Vector2Int> validPositions = new List<Vector2Int>();

	public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos, List<Vector2Int> validPositions) {
        this.validPositions = validPositions;

        Node startNode = new Node(startPos, validPositions);
		Node targetNode = new Node(targetPos, validPositions);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode) {
				return RetracePath(startNode,targetNode);;
			}

			foreach (Node neighbour in GetNeighbours(node)) {
				if (!neighbour.validPos || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

        return null;
	}

	List<Vector2Int> RetracePath(Node startNode, Node endNode) {
		List<Vector2Int> path = new List<Vector2Int>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode.pos);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		return path;

	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.pos.x - nodeB.pos.x);
		int dstY = Mathf.Abs(nodeA.pos.y - nodeB.pos.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                neighbours.Add(new Node(new Vector2Int(node.pos.x+x, node.pos.y+y), validPositions));
            }
        }

        return neighbours;
    }
}
