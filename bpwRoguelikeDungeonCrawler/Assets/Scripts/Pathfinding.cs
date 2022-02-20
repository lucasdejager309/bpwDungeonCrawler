using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public Vector2Int pos;
    public int gCost;
    public int hCost;
    public Node previousInPath;
    
    public Node(Vector2Int pos) {
        this.pos = pos;
    }
    
    public int fCost {
        get {
            return gCost+hCost;
        }
    }

    public static int GetDistance(Vector2Int posA, Vector2Int posB) {
        int dstX = Mathf.Abs(posA.x - posB.x);
        int dstY = Mathf.Abs(posA.y - posB.y);

        if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
    }
}

public class Pathfinding
{
    public static List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }
                neighbours.Add(new Node(new Vector2Int(node.pos.x+x, node.pos.y+y)));
            }
        }

        return neighbours;
    }

    public static List<Vector2Int> GetPath(Vector2Int startPos, Vector2Int endPos, List<Vector2Int> traversablePositions) {
        Node startNode = new Node(startPos);
        Node targetNode = new Node(endPos);
        
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        //add start node to openNodes
        openNodes.Add(startNode);

        while(openNodes.Count > 0) {
            //currentNode = node in openNodes with lowest fCost
            Node currentNode = openNodes[0];
            foreach (Node node in openNodes) {
                if (node.fCost < currentNode.fCost || node.fCost == currentNode.fCost) {
                    if (node.hCost < currentNode.hCost) {
                        currentNode = node;
                    }
                }
            }

            //remove current node from openNodes, add to closedNodes
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode == targetNode) {
                Debug.Log("found path :)");
                return RetracePath(startNode, targetNode);
            }

            List<Node> currentNeighbours = GetNeighbours(currentNode);
            foreach (Node neighbour in currentNeighbours) {
                //if neighbour isnt traversable or already checked
                if (!traversablePositions.Contains(neighbour.pos) || closedNodes.Contains(neighbour)) {
                    continue;
                } 
                
                //if path to neighbour is shorter or neighbour is not in open
                int newCostToNeighbour = currentNode.gCost + Node.GetDistance(currentNode.pos, neighbour.pos);
                if (newCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour)) {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Node.GetDistance(neighbour.pos, targetNode.pos);
                    neighbour.previousInPath = currentNode;

                    if (!openNodes.Contains(neighbour)) {
                        openNodes.Add(neighbour);
                    }
                }
            }
        }
        Debug.Log("didnt find path :(");
        return null;
    }

    static List <Vector2Int> RetracePath(Node startNode, Node endNode) {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode.pos);
            currentNode = currentNode.previousInPath;
        }

        path.Reverse();
        return path;
    }
}
