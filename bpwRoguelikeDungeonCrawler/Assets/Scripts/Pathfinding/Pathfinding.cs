using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Pathfinding {

    public static List<PathNode> FindPath(PathNode startNode, PathNode endNode, List<PathNode> allowedNodes) {
		List<PathNode>openList = new List<PathNode>() { startNode };
		List<PathNode>closedList = new List<PathNode>();

		startNode.gCost = 0;
		startNode.hCost = PathNode.CalculateDistance(startNode, endNode);

		while (openList.Count > 0) {
			PathNode currentNode = PathNode.GetLowestFcostNode(openList);

			if (currentNode.pos == endNode.pos) {
				Debug.Log(closedList.Count);
				return CalculatePath(currentNode);
			}

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			foreach(PathNode neighbourNode in currentNode.GetNeighbouringNodes(allowedNodes)) {
				if (closedList.Contains(neighbourNode)) continue;

				int tentativeGCost = currentNode.gCost + PathNode.CalculateDistance(currentNode, neighbourNode);
				if (tentativeGCost < neighbourNode.gCost) {
					neighbourNode.cameFromNode = currentNode;

					neighbourNode.gCost = tentativeGCost;
					neighbourNode.hCost = PathNode.CalculateDistance(neighbourNode, endNode);


					if (!openList.Contains(neighbourNode)) {
						openList.Add(neighbourNode);
					}
				}
			}
		}

		return null;
	}

	private static List<PathNode> CalculatePath(PathNode endNode) {
		List<PathNode> path = new List<PathNode>();

		path.Add(endNode);
		PathNode currentNode = endNode;
		while (currentNode.cameFromNode != null) {
			path.Add(currentNode);
			currentNode = currentNode.cameFromNode;
		}

		path.Reverse();
		return path;
	}
}
