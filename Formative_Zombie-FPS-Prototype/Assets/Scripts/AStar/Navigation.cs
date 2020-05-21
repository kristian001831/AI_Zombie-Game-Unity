using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour // AStar Algorithm
{
    private NavGrid navGridRef;
    public NavGrid NavGridRef
    {
        get => navGridRef;
        set => navGridRef = value;
    }

    private Transform origin;

    // its a test for now.... TODO
    public static List<Transform> Origins = new List<Transform>();
    
    [SerializeField] private Transform target;

    private void Awake()
    {
        navGridRef = GetComponent<NavGrid>();
    }

    private void LateUpdate()
    {
        // do this for every origin (zombie in the scene)
        for(int i = 0; i < Origins.Count; i++)
        {
            origin = Origins[i];
            Debug.Log(Origins[i]);
            SolvePathBetween(origin.position, target.position);    
        }
        // solve the path
        //SolvePathBetween(origin.position, target.position);
    }

    public void SolvePathBetween(Vector3 originPosition, Vector3 targetPosition)
    {
        // Reference to origin node and target node
        Node originNode = navGridRef.GetNodeFromWorldCoord(originPosition);
        Node targetNode = navGridRef.GetNodeFromWorldCoord(targetPosition);
        
        // creating two empty lists to hold our open and closed nodes
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();
        
        // first off, we add the origin node to the open list
        openNodes.Add(originNode);
        
        // loop trought all the nodes and make sure that the currently examined node is the one with the lowest cost (f cost)
        while (openNodes.Count > 0)
        {
            // get a reffernce to the currently examinde node and set it to the first node in the open list
            Node examinedNode = openNodes[0];
            
            // loop trough the open nodes and make sure the examined node is the best one
            for (int n = 1; n < openNodes.Count; n++)
            {
                // its better when, it either has a lower F cost or if the F cost is the same, its better when it has a lower H cost
                if (openNodes[n].GetTotalCost() < examinedNode.GetTotalCost() ||
                    openNodes[n].GetTotalCost() == examinedNode.GetTotalCost() &&
                    openNodes[n].CostToTarget < examinedNode.CostToTarget)
                {
                    examinedNode = openNodes[n];
                }
            }
            
            // remove the examined node from the open list and add it to the closed list
            openNodes.Remove(examinedNode);
            closedNodes.Add(examinedNode);

            // check if we found the target node
            if (examinedNode == targetNode)
            {
                // we must have the path 
                navGridRef.solvedPath = GetPath(originNode, targetNode);
                return; // TODO: return the actual path
            }

            foreach (Node adjacentNode in GetAdjacentNodesTo(examinedNode)) 
            {
                if (!adjacentNode.Valid || closedNodes.Contains(adjacentNode))
                {
                    continue;
                }

                // cost to travel to the adjacent node is the distance from origin to examined + the distance from examined to adjacent
                int costToTravelToAdjacent = examinedNode.CostToOrigin + GetTravelDistanceBetween(examinedNode, adjacentNode);

                if (costToTravelToAdjacent < adjacentNode.CostToOrigin || !openNodes.Contains(adjacentNode))
                {
                    // the cost to origin is actually less through the examined node
                    adjacentNode.CostToOrigin = costToTravelToAdjacent;
                    adjacentNode.CostToTarget = GetTravelDistanceBetween(adjacentNode, targetNode);
                    adjacentNode.Parent = examinedNode; // used nodes

                    // if the adjacent node is not in the open list, add it
                    if (!openNodes.Contains(adjacentNode))
                    {
                        openNodes.Add(adjacentNode);
                    }
                }
            }
        }
    }

    private List<Node> GetAdjacentNodesTo(Node node) // Adjecent == next too
    {
        List<Node> adjacentNodes = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int adjacentX = node.GridCoordinateX + x;
                int adjacentY = node.GridCoordinateY + y;

                if (adjacentX >= 0 && adjacentX < navGridRef.GridNodeSize.x && adjacentY >= 0 &&
                    adjacentY < navGridRef.GridNodeSize.y)
                {
                    adjacentNodes.Add(navGridRef.NavGridV[adjacentX, adjacentY]);
                }
            }
        }

        return adjacentNodes;
    }

    // int is more efficient than float (int = 10/14 float= 1/1,4)
    private int GetTravelDistanceBetween(Node startNode, Node endNode)
    {
        int xDistance = Mathf.Abs(startNode.GridCoordinateX - endNode.GridCoordinateX);
        int yDistance = Mathf.Abs(startNode.GridCoordinateY - endNode.GridCoordinateY);

        return xDistance > yDistance ? yDistance * 14 + 10 * (xDistance - yDistance) : xDistance * 14 + 10 * (yDistance - xDistance);
    }

    // Get a final path to the target 
    private List<Node> GetPath(Node originNode, Node targetNode)
    {
        List<Node> solvedPath = new List<Node>();

        // start at the end
        Node currentNode = targetNode;
        
        // loop trough as long as we have not gotten to the origin
        while (currentNode != originNode)
        {
            // add the current node and set the new current node to be its parent
            solvedPath.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        
        // reverse the path 
        solvedPath.Reverse();

        return solvedPath;
    }
}
