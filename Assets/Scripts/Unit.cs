using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Unit : MonoBehaviour
{
    private Pathfinding pathfinding;
    private GridXZ<PathNode> grid;
    public Transform target;
    float speed=5f;
    float rotationSpeed=5f;
    Vector3[] path;
    int targetIndex;
    
    private void Awake()
    {
        GameObject aStar=GameObject.Find("A*");
        pathfinding = aStar.GetComponent<Pathfinding>();
        
    }

    private void Start()
    {
        grid = pathfinding.GetGrid();
        this.transform.position = new Vector3(-90, 0,-90)+new Vector3(grid.GetCellSize()*.5f,grid.GetCellSize()*.5f);
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            PathRequestManager.RequestPath(transform.position, mouseWorldPosition, OnPathFound);
            

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            GridXZ<PathNode> pathNodeGrid = grid;
            pathNodeGrid.GetXZ(mouseWorldPosition, out int x, out int z);
            PathNode updateNode = pathfinding.GetNode(x, z);
            if (updateNode != null)
            {
                updateNode.SetIsWalkable(!updateNode.isWalkable);
                pathfinding.UpdateRegionNeighbour(updateNode);
            }


        }
    }

    public void OnPathFound(Vector3[] newPath,bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;

            if (pathfinding.calculatedPath != null)
            {
                for (int i = 0; i < pathfinding.calculatedPath.Count - 1; i++)
                {
                    UnityEngine.Debug.DrawLine(grid.GetWorldPosition(pathfinding.calculatedPath[i].GetX(), pathfinding.calculatedPath[i].GetZ()) + .5f * grid.GetCellSize() * Vector3.one, grid.GetWorldPosition(pathfinding.calculatedPath[i + 1].GetX(), pathfinding.calculatedPath[i + 1].GetZ()) + .5f * grid.GetCellSize() * Vector3.one, Color.green, 1000f);
                }
            }

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");

        }
    }

    IEnumerator FollowPath()
    {
        targetIndex = 0;
        Vector3 currentWayPoint = path[0];
        while (true)
        {
            if (transform.position == currentWayPoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    path = new Vector3[0];
                    yield break;
                }
                currentWayPoint = path[targetIndex];
            }
            //Rotate Towards Next Waypoint
            Vector3 targetDir = currentWayPoint - this.transform.position;
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position = Vector3.MoveTowards(transform.position,currentWayPoint,speed*Time.deltaTime);
            yield return null;
        }
    }
}
