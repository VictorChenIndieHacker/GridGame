using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Unit : MonoBehaviour
{
    private Pathfinding pathfinding;
    private Grid<PathNode> grid;
    public Transform target;
    float speed=50f;
    Vector3[] path;
    int targetIndex;
    private bool takePlayerInput=false;
    private void Awake()
    {
        GameObject aStar=GameObject.Find("A*");
        pathfinding = aStar.GetComponent<Pathfinding>();
        
    }

    private void Start()
    {
        grid = pathfinding.GetGrid();
        this.transform.position = Vector3.zero;
        target.position = grid.GetWorldPosition(49,36);
        PathRequestManager.RequestPath(transform.position,target.position,OnPathFound);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            takePlayerInput = !takePlayerInput;
        }
        if (!takePlayerInput) return;

        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
        if (Input.GetMouseButtonDown(0))
        {

            PathRequestManager.RequestPath(transform.position, mouseWorldPosition, OnPathFound);
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
                    UnityEngine.Debug.DrawLine(grid.GetWorldPosition(pathfinding.calculatedPath[i].GetX(), pathfinding.calculatedPath[i].GetY()) + .5f * grid.GetCellSize() * new Vector3(1,1,0), grid.GetWorldPosition(pathfinding.calculatedPath[i + 1].GetX(), pathfinding.calculatedPath[i + 1].GetY()) + .5f * grid.GetCellSize() * new Vector3(1,1,0), Color.green, 1000f);
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
            

            transform.position = Vector3.MoveTowards(transform.position,currentWayPoint,speed*Time.deltaTime);
            yield return null;
        }
    }
}
