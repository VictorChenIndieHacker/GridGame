using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode:IHeapItem<PathNode>
{
    private Grid<PathNode> grid;
    private int x;
    private int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;
    int heapIndex;

    public List<PathNode> neighbours;

    public PathNode(Grid<PathNode> grid,int x,int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
        neighbours = new List<PathNode>();
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x+","+y;
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
}
