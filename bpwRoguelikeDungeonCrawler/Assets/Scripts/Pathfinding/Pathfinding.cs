using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR THIS SCRIPT I USED THE FOLLOWING TUTORIALS:
//A* Pathfinding Tutorial by Sebastian Lague: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
//A* Pathfinding in Unity by Code Monkey: https://www.youtube.com/watch?v=alU04hvz6L4 

public static class Pathfinding {

    public static List<PathNode> FindPath(PathNode startNode, PathNode endNode, List<Vector2Int> allowedPositions) {
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

			foreach(PathNode neighbourNode in currentNode.GetNeighbouringNodes(allowedPositions)) {
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
