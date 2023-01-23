using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils; 
public class GridBuildingSystem : MonoBehaviour
{
    private Pathfinding pathfinding;
    private GridXZ<PathNode> grid;
    private Transform buildingOnMouse;
    [SerializeField] private bool isBuilding=false;
    [SerializeField] private LayerMask mouseColliderMask;
    [SerializeField] private PlacedObjectTypeSO placedObjectTypeSO;
    private void Awake()
    {
        GameObject aStar = GameObject.Find("A*");
        pathfinding = aStar.GetComponent<Pathfinding>();

    }

    private void Start()
    {
        grid = pathfinding.GetGrid();
        
    }

    private void Update()
    {
        if (!isBuilding) return;
        Vector3 mouseWorldPosition = UtilsClassTMP.GetMouseWorldPosition3D(mouseColliderMask);
        grid.GetXZ(mouseWorldPosition,out int x,out int z);
        PathNode mouseNode = grid.GetGridObject(x, z);
        if ( mouseNode== null) return;
        Vector3 mouseNodeposition=grid.GetWorldPosition(mouseNode.GetX(), mouseNode.GetZ());
        if (buildingOnMouse == null)
        {
            buildingOnMouse = Instantiate(placedObjectTypeSO.prefab, mouseNodeposition, Quaternion.identity);

        }
        buildingOnMouse.position = mouseNodeposition;
        if (Input.GetMouseButtonDown(0))
        {
            if (mouseNode.CanBuild())
            {
                mouseNode.SetTransform(buildingOnMouse);
                mouseNode.SetIsWalkable(false);
                buildingOnMouse = null;
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Cannot buid here!", mouseNodeposition);
            }
            
        }
    }
}
