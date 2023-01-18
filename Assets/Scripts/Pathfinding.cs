using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public class Pathfinding:MonoBehaviour
{

    private int width=100;
    private int height=100;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private GridXZ<PathNode> grid;
    private Heap<PathNode> openList;
    private bool[,] closedSet;
    public List<PathNode> calculatedPath;
    PathRequestManager requestManager;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = new GridXZ<PathNode>(width, height, 1f, new Vector3(-90, 0,-90), (GridXZ<PathNode> g, int x, int z) => new PathNode(g, x, z));
        openList = new Heap<PathNode>(grid.GetWidth() * grid.GetHeight());
        calculatedPath = new List<PathNode>();
        for (int z = 85; z < 94; z++)
        {
            grid.GetGridObject(98,z).isWalkable = false;
        }
        grid.GetGridObject(99, 85).isWalkable = false;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                SetNeighbourList(grid.GetGridObject(x, z));
            }
        }

        
    }

    public void StartFindPath(Vector3 startPos,Vector3 endPos)
    {
        grid.GetXZ(startPos, out int startX, out int startZ);
        grid.GetXZ(endPos,out int endX,out int endZ);
        print("My start grid is: "+startX+","+ startZ);
        print("My destination grid is" + endX + "," + endZ);
        StartCoroutine(FindPath(startX, startZ, endX, endZ));
    }

    IEnumerator FindPath(int startX,int startZ,int endX,int endZ)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        calculatedPath.Clear();
        PathNode startNode =grid.GetGridObject(startX, startZ);
        PathNode endNode = grid.GetGridObject(endX, endZ);
        if (startNode != null && endNode != null && startNode.isWalkable && endNode.isWalkable)
        {
            openList.Clear();
            closedSet = new bool[grid.GetWidth(), grid.GetHeight()];
            openList.Add(startNode);

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int z = 0; z < grid.GetHeight(); z++)
                {
                    PathNode pathNode = grid.GetGridObject(x, z);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = openList.RemoveFirst();
                if (currentNode == endNode)
                {
                    //reached final node
                    stopwatch.Stop();
                    print("Path found: " + stopwatch.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }

                closedSet[currentNode.GetX(), currentNode.GetZ()] = true;
                foreach (PathNode neighbourNode in currentNode.neighbours)
                {
                    if (closedSet[neighbourNode.GetX(), neighbourNode.GetZ()]) continue;
                    if (!neighbourNode.isWalkable)
                    {
                        closedSet[neighbourNode.GetX(), neighbourNode.GetZ()] = true;
                        continue;
                    }

                    int tentativeGcost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                    if (tentativeGcost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGcost;
                        neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();
                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                        else
                        {
                            openList.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints= CalculatePath(endNode);
        }
        requestManager.FinishProcessingPath(waypoints, pathSuccess);
    }


    void SetNeighbourList(PathNode currentNode)
    {

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }
                
                int checkX = currentNode.GetX() + x;
                int checkZ = currentNode.GetZ() + z;

                if (checkX >= 0 && checkX < grid.GetWidth() && checkZ >= 0 && checkZ < grid.GetHeight())
                {
                    if(Mathf.Abs(x)==1&&Mathf.Abs(z)==1&&!GetNode(checkX,currentNode.GetZ()).isWalkable|| Mathf.Abs(x) == 1 && Mathf.Abs(z) == 1 && !GetNode(currentNode.GetX(), checkZ).isWalkable)
                    {
                        continue;
                    }
                    currentNode.neighbours.Add(GetNode(checkX, checkZ));
                }
            }
        }
    }


    public void UpdateRegionNeighbour(PathNode updatedNode)
    {

        foreach (PathNode neighbour in updatedNode.neighbours)
        {
            if (neighbour.isWalkable)
            {
                neighbour.neighbours.Clear();
                SetNeighbourList(neighbour);
            }
        }
    }

    public GridXZ<PathNode> GetGrid()
    {
        return grid;
    }

    public PathNode GetNode(int x,int z)
    {
        return grid.GetGridObject(x, z);
    }

    Vector3[] CalculatePath(PathNode endNode)
    {
        calculatedPath.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            calculatedPath.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        Vector3[] waypoints=SimplifyPath(calculatedPath);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<PathNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 0; i < path.Count-1; i++)
        {
            Vector2 directionNew = new Vector2(path[i].GetX() - path[i+1].GetX(), path[i].GetZ() - path[i+1].GetZ());
            if (directionNew != directionOld)
            {
                waypoints.Add(grid.GetWorldPosition(path[i].GetX(), path[i].GetZ())+new Vector3(grid.GetCellSize()*.5f,grid.GetCellSize()*.5f));
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    private int CalculateDistance(PathNode a,PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int zDistance = Mathf.Abs(a.GetZ() - b.GetZ());
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    
}
