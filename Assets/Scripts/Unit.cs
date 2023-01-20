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
    float speed=50f;
    float rotationSpeed=30f;
    Vector3[] path;
    int targetIndex;
    bool IsInGroundPlane;
    [SerializeField] private LayerMask mouseColliderLayerMask;
    [SerializeField] private Transform mouseVisualTransform;
    private void Awake()
    {
        GameObject aStar=GameObject.Find("A*");
        pathfinding = aStar.GetComponent<Pathfinding>();
        
    }

    private void Start()
    {
        grid = pathfinding.GetGrid();
        this.transform.position = new Vector3(-50, 0,-50)+new Vector3(grid.GetCellSize()*.5f,0,grid.GetCellSize()*.5f);
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f,mouseColliderLayerMask))
        {
            IsInGroundPlane = true;
            mouseVisualTransform.position = raycastHit.point;
            mouseVisualTransform.gameObject.SetActive(true);
                
        }
        else
        {
            IsInGroundPlane = false;
            mouseVisualTransform.gameObject.SetActive(false);
        }
        
        if (Input.GetMouseButtonDown(0)&&IsInGroundPlane)
        {
            PathRequestManager.RequestPath(transform.position, mouseVisualTransform.position, OnPathFound);
        }

        if (Input.GetMouseButtonDown(1)&&IsInGroundPlane)
        {
            GridXZ<PathNode> pathNodeGrid = grid;
            pathNodeGrid.GetXZ(mouseVisualTransform.position, out int x, out int z);
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
                    UnityEngine.Debug.DrawLine(grid.GetWorldPosition(pathfinding.calculatedPath[i].GetX(), pathfinding.calculatedPath[i].GetZ()) + .5f * grid.GetCellSize() * new Vector3(1,0,1), grid.GetWorldPosition(pathfinding.calculatedPath[i + 1].GetX(), pathfinding.calculatedPath[i + 1].GetZ()) + .5f * grid.GetCellSize() * new Vector3(1,0,1), Color.green, 1000f);
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
