using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public class Pathfinding:MonoBehaviour
{

    private int width=50;
    private int height=50;
    private float cellSize = 10f;
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private Grid<PathNode> grid;
    private Heap<PathNode> openList;
    private bool[,] closedSet;
    public List<PathNode> calculatedPath;
    PathRequestManager requestManager;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = new Grid<PathNode>(width, height, cellSize, Vector2.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        openList = new Heap<PathNode>(grid.GetWidth() * grid.GetHeight());
        calculatedPath = new List<PathNode>();
        for (int y = 35; y < 44; y++)
        {
            grid.GetGridObject(48,y).isWalkable = false;
        }
        grid.GetGridObject(49, 35).isWalkable = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetNeighbourList(grid.GetGridObject(x, y));
            }
        }

        
    }

    public void StartFindPath(Vector3 startPos,Vector3 endPos)
    {
        grid.GetXY(startPos, out int startX, out int startY);
        grid.GetXY(endPos,out int endX,out int endY);
        print("My start grid is: "+startX+","+ startY);
        print("My destination grid is" + endX + "," + endY);
        StartCoroutine(FindPath(startX, startY, endX, endY));
    }

    IEnumerator FindPath(int startX,int startY,int endX,int endY)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        calculatedPath.Clear();
        PathNode startNode =grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        if (startNode != null && endNode != null&&!PathNode.CheckIfEqual(startNode,endNode) && endNode.isWalkable)
        {
            openList.Clear();
            closedSet = new bool[grid.GetWidth(), grid.GetHeight()];
            openList.Add(startNode);

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    PathNode pathNode = grid.GetGridObject(x, y);
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

                closedSet[currentNode.GetX(), currentNode.GetY()] = true;
                foreach (PathNode neighbourNode in currentNode.neighbours)
                {
                    if (closedSet[neighbourNode.GetX(), neighbourNode.GetY()]) continue;
                    if (!neighbourNode.isWalkable)
                    {
                        closedSet[neighbourNode.GetX(), neighbourNode.GetY()] = true;
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
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                
                int checkX = currentNode.GetX() + x;
                int checkY = currentNode.GetY() + y;

                if (checkX >= 0 && checkX < grid.GetWidth() && checkY >= 0 && checkY < grid.GetHeight())
                {
                    if(Mathf.Abs(x)==1&&Mathf.Abs(y)==1&&!GetNode(checkX,currentNode.GetY()).isWalkable|| Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1 && !GetNode(currentNode.GetX(), checkY).isWalkable)
                    {
                        continue;
                    }
                    currentNode.neighbours.Add(GetNode(checkX, checkY));
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

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public PathNode GetNode(int x,int y)
    {
        return grid.GetGridObject(x, y);
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
            Vector2 directionNew = new(path[i].GetX() - path[i+1].GetX(), path[i].GetY() - path[i+1].GetY());
            if (directionNew != directionOld)
            {
                waypoints.Add(grid.GetWorldPosition(path[i].GetX(), path[i].GetY()));
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    private int CalculateDistance(PathNode a,PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    
}
