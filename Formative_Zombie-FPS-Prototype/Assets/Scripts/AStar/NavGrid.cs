using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    public Vector2Int GridNodeSize; // number of x nodes and number of y nodes
    public Node[,] NavGridV; // 2D Array representing the nav grid
    public float NodeSize; // how big (real world unit size) the space between nodes is
    
    // Set up Layer masks
    public LayerMask BlockingMask;
    public LayerMask SandMask;
    public LayerMask WaterMask;

    public List<Node> solvedPath;

    public bool ShowDebugGizmos = true;

    private void Start()
    {
        CreateNavGrid();
    }

    // greates an NavGrid (list of nodes)
    private void CreateNavGrid()
    {
        // create a new 2D array with the correct size (number of members)
        NavGridV = new Node[GridNodeSize.x, GridNodeSize.y];

        int CostMultiplier = 1;
        
        //loop trought x and y and add nodes to the list  (navGrid)
        for (int x = 0; x < GridNodeSize.x; x++)
        {
            for (int y = 0; y < GridNodeSize.y; y++)
            {
                // get a real world location of this node
                Vector3 realWorldNodeLocation = new Vector3(x * NodeSize, transform.position.y,y * NodeSize)
                    + transform.position;
                
                // check if the node is blocked
                bool nodeIsBlocked = Physics.CheckSphere(realWorldNodeLocation, NodeSize / 2, BlockingMask);

                // check if the node is on sand
                CostMultiplier = Physics.CheckSphere(realWorldNodeLocation, NodeSize / 2, SandMask) ? 2 : 1;
                
                // check if the node is on water
                CostMultiplier = Physics.CheckSphere(realWorldNodeLocation, NodeSize / 2, WaterMask) ? 3 : CostMultiplier;
                
                // add the node to the navGrid
                NavGridV[x, y] = new Node(!nodeIsBlocked, x, y, CostMultiplier);
            }
        }
    }

    private Vector3 GetRealWorldCoordFromGridLocation(Vector2 location)
    {
        return new Vector3(location.x * NodeSize, 0, location.y * NodeSize) + transform.position;
    }

    public Node GetNodeFromWorldCoord(Vector3 coord)
    {
        //Fix for my problem
        //fetch pos in relation to grid length & width
        float percentX = (coord.x + GridNodeSize.x / 2) / GridNodeSize.x;
        float percentY = (coord.z + GridNodeSize.y / 2) / GridNodeSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Round to int for grid pos
        int gridX = Mathf.RoundToInt((GridNodeSize.x - 1) * percentX);
        int gridY = Mathf.RoundToInt((GridNodeSize.y - 1) * percentY);

        return NavGridV[gridX, gridY];
    }

    // Visualize the nodes using Gizmos
    public void OnDrawGizmos()
    {
        // return if the nav grid hasnt been initilized
        if (NavGridV == null || !ShowDebugGizmos) return;

        // loop trought each node and color it and create a sphere gizmos
        foreach (Node node in NavGridV)
        {
            // By default it should be white
            Gizmos.color = Color.white;
            
            // if its blocked, make it red
            if (!node.Valid)
            {
                Gizmos.color = Color.red;
            }
            else if (node.CostMultiplier == 2) // must be sand
            {
                Gizmos.color = Color.yellow;
            }
            else if (node.CostMultiplier == 3) // must be water
            {
                Gizmos.color = Color.blue;
            }

            if (solvedPath.Contains(node))
            {
                Gizmos.color = Color.black;
            }

            Gizmos.DrawSphere(
                GetRealWorldCoordFromGridLocation(new Vector2(node.GridCoordinateX, node.GridCoordinateY)),
                NodeSize / 3);
        }
    }
}