using System.Collections.Generic;

public class PathNode:IHeapItem<PathNode>
{
    private Grid<PathNode> grid;
    private int x;
    private int y;
    private PlacedObject placedObject;

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
        return x+","+y+"\n"+placedObject;
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
        if(a.x==b.x&&a.y==b.y)
        {
            return true;
        }
        return false;
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        this.placedObject = placedObject;
        grid.TriggerGridObjectChanged(x,y);
    }

    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }

    public void ClearPlacedObject()
    {
        placedObject = null;
        grid.TriggerGridObjectChanged(x, y);
    }

    public bool CanBuild()
    {
        return placedObject == null;
    }
}
