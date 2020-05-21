using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int CostToTarget; // H Cost (heristic)
    /*public int CostToTarget // better to use in c# but would maybe not work in c++, etc
    {
        get => costToTarget;
        set => costToTarget = value;
    }*/

    public int CostToOrigin; // G Cost (cost to parent)
    
    // is the node on a blocking geometry. If so valid = false;
    public bool Valid;
    
    // is this node on sand or water? If so, set a cost multiplier to simulate a slower traversal
    public int CostMultiplier;

    // keep track of its grid (array) location
    public int GridCoordinateX;
    public int GridCoordinateY;

    // refernece to the parent node of its node
    public Node Parent;

    // return the cost to the node traversal
    public int GetTotalCost() // F Cost
    {
        return (CostToTarget + CostToOrigin) * CostMultiplier;
    }

    // constructor
    public Node(bool valid, int gridCoordinateX, int gridCoordinateY, int costMultiplier = 1)
    {
        // set the properties of this node to the constructor
        Valid = valid;
        GridCoordinateX = gridCoordinateX;
        GridCoordinateY = gridCoordinateY;
        CostMultiplier = costMultiplier;
    }
}
