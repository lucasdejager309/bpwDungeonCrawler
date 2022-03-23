using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//FOR THIS SCRIPT I USED THE FOLLOWING TUTORIALS:
//A* Pathfinding Tutorial by Sebastian Lague: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW
//A* Pathfinding in Unity by Code Monkey: https://www.youtube.com/watch?v=alU04hvz6L4 

public static class Pathfinding {

    public static List<PathNode> FindPath(PathNode startNode, PathNode endNode, List<Vector2Int> allowedPositions) {	
		//add pos of entity to validnodePositions (otherwise pathfinding is like huh no valid position)
        List<Vector2Int> validNodePositions = allowedPositions;
        validNodePositions.Add(startNode.pos);
        validNodePositions.Add(endNode.pos);
		
		Heap<PathNode>openHeap = new Heap<PathNode>(1000000);
		List<PathNode>closedList = new List<PathNode>();

		openHeap.Add(startNode);

		startNode.gCost = 0;
		startNode.hCost = PathNode.CalculateDistance(startNode, endNode);

		while (openHeap.Count > 0) {
			PathNode currentNode = openHeap.RemoveFirst();

			if (currentNode.pos == endNode.pos) {
				return CalculatePath(currentNode);
			}

			closedList.Add(currentNode);

			foreach(PathNode neighbourNode in currentNode.GetNeighbouringNodes(validNodePositions)) {
				if (closedList.Contains(neighbourNode)) continue;

				int tentativeGCost = currentNode.gCost + PathNode.CalculateDistance(currentNode, neighbourNode);
				if (tentativeGCost < neighbourNode.gCost) {
					neighbourNode.cameFromNode = currentNode;

					neighbourNode.gCost = tentativeGCost;
					neighbourNode.hCost = PathNode.CalculateDistance(neighbourNode, endNode);

					//TODO: THIS STATEMENT NEVER RETURNS FALSE
					if (!openHeap.Contains(neighbourNode)) {
						openHeap.Add(neighbourNode);
					}
				}
			}

			if (openHeap.Count > 1000) {
				break;
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
