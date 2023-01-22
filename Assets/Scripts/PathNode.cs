using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode:IHeapItem<PathNode>
{
    private GridXZ<PathNode> grid;
    private int x;
    private int z;
    private Transform transform;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;
    int heapIndex;

    public List<PathNode> neighbours;

    public PathNode(GridXZ<PathNode> grid,int x,int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
        isWalkable = true;
        neighbours = new List<PathNode>();
    }

    public int GetX()
    {
        return x;
    }

    public int GetZ()
    {
        return z;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x+","+z+"\n"+transform;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(PathNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    public static bool CheckIfEqual(PathNode a, PathNode b)
    {
        if(a.x==b.x&&a.z==b.z)
        {
            return true;
        }
        return false;
    }

    public void SetTransform(Transform transform)
    {
        this.transform = transform;
        grid.TriggerGridObjectChanged(x,z);
    }

    public void ClearTransform()
    {
        transform = null;
        grid.TriggerGridObjectChanged(x, z);
    }

    public bool CanBuild()
    {
        return transform == null;
    }
}
