using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Pathfinding pathfinding;
    private Grid<PathNode> grid;
    // Start is called before the first frame update
    void Start()
    {
        GameObject aStar = GameObject.Find("A*");
        pathfinding = aStar.GetComponent<Pathfinding>();
        grid = pathfinding.GetGrid();
        this.transform.position = grid.GetWorldPosition(99, 86) + new Vector3(grid.GetCellSize()*.5f,grid.GetCellSize()*.5f);
          
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
