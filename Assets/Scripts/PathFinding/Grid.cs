using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkable;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Start() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeX; y++) {
                // Get each point that is occupied by a node in the world
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkable));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node currentNode) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }

                int checkX = currentNode.gridX + x;
                int checkY = currentNode.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float gridPosX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float gridPosY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        gridPosX = Mathf.Clamp01(gridPosX);
        gridPosY = Mathf.Clamp01(gridPosY);

        // Minus 1, so we're not going outside of the array
        int x = Mathf.RoundToInt((gridSizeX - 1) * gridPosX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * gridPosY);

        return grid[x, y];
    }

    public List<Node> path;
    
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null) {
            foreach (Node node in grid) {
                Gizmos.color = (node.walkable) ? Color.white : Color.red;
                if (path != null) {
                    if (path.Contains(node)) {
                        Gizmos.color = Color.black;
                    }
                }

                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
