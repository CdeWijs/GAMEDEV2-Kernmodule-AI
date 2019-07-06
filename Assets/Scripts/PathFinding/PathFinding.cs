using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinding : MonoBehaviour
{
    private PathRequestManager requestManager;
    private Grid grid;

    private void Awake() {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // Calculate nodes from positions
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable) {
            List<Node> openNodes = new List<Node>(); // the set of nodes to be evaluated
            HashSet<Node> closedNodes = new HashSet<Node>(); // the set of nodes already evaluated
            openNodes.Add(startNode);

            while (openNodes.Count > 0) {
                Node currentNode = openNodes[0];
                for (int i = 0; i < openNodes.Count; i++) {
                    // current node = node in Open Nodes with the lowest F_cost, or (when the highest open node has the same F_cost as the current node) get the node with the highest H_cost
                    if (openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost) {
                        currentNode = openNodes[i];
                    }
                }

                // Remove current from openNodes and add to closedNodes
                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                if (currentNode == targetNode) {
                    // Found path
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                    if (!neighbour.walkable || closedNodes.Contains(neighbour)) {
                        // Skip to the next neighbour
                        continue;
                    }

                    // Check if the new path to the neighbour is shorter or if the neighbour is not in Open nodes
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openNodes.Contains(neighbour)) {
                            openNodes.Add(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    // Get the path from the start node to the target node by going backwards.
    private Vector3[] RetracePath(Node startNode, Node targetNode) {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        grid.path = path;

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i <  path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld) {
                waypoints.Add(path[i - 1].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB) {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // get x axis
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // get y axis

        // Take the lowest axis, to know how many diagonal moves it takes to be either horizontally or vertically in line with the target node (nodeB).
        // To get the remaining number of horizontal or vertical moves, subtract the lowest axis from the highest.
        // (14 * lowest) + 10 * (highest - lowest)
        if (distanceX > distanceY) {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        } else {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
