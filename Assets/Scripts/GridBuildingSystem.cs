using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils; 
public class GridBuildingSystem : MonoBehaviour
{
    private Pathfinding pathfinding;
    private GridXZ<PathNode> grid;
    private PlacedObjectTypeSO.Dir dir=PlacedObjectTypeSO.Dir.Down;
    private Transform buildingOnMouse;
    [SerializeField] private Material transparentMat;
    [SerializeField] private Material opaqueMat;
    [SerializeField] private bool isBuilding=false;
    [SerializeField] private LayerMask mouseColliderMask;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;
    private void Awake()
    {
        GameObject aStar = GameObject.Find("A*");
        pathfinding = aStar.GetComponent<Pathfinding>();

        placedObjectTypeSO = placedObjectTypeSOList[0];
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
        List<Vector2Int> gridPositionList=placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z),dir);
        PathNode mouseNode = grid.GetGridObject(x, z);
        if ( mouseNode== null) return;
        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
        Vector3 mouseNodeposition =grid.GetWorldPosition(mouseNode.GetX(), mouseNode.GetZ())+new Vector3(rotationOffset.x,0,rotationOffset.y)*grid.GetCellSize();
        if (buildingOnMouse == null)
        {
            buildingOnMouse = Instantiate(placedObjectTypeSO.prefab, mouseNodeposition, Quaternion.Euler(0,placedObjectTypeSO.GetRotationAngle(dir),0));

        }
        Color color_MouseOnBuilding = buildingOnMouse.GetChild(0).GetComponent<MeshRenderer>().material.color;
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!grid.GetGridObject(gridPosition.x,gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        if (canBuild&&color_MouseOnBuilding.g!=1&&color_MouseOnBuilding.b!=1)
        {
            color_MouseOnBuilding.g = 1;
            color_MouseOnBuilding.b = 1;
            buildingOnMouse.GetChild(0).GetComponent<MeshRenderer>().material.color = color_MouseOnBuilding;
        }

        if (!canBuild&&color_MouseOnBuilding.g!=0&&color_MouseOnBuilding.b!=0)
        {
            color_MouseOnBuilding.g = 0;
            color_MouseOnBuilding.b = 0;
            buildingOnMouse.GetChild(0).GetComponent<MeshRenderer>().material.color = color_MouseOnBuilding;

        }
        buildingOnMouse.position = mouseNodeposition;
        buildingOnMouse.rotation = Quaternion.Euler(0,placedObjectTypeSO.GetRotationAngle(dir),0);
        if (Input.GetMouseButtonDown(0))
        {
            if (canBuild)
            {
                buildingOnMouse.GetChild(0).GetComponent<MeshRenderer>().material = opaqueMat;
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(buildingOnMouse);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetIsWalkable(false);
                }

                buildingOnMouse = null;
            }
            
            
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placedObjectTypeSO = placedObjectTypeSOList[0];
            Destroy(buildingOnMouse.gameObject);
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placedObjectTypeSO = placedObjectTypeSOList[1];
            Destroy(buildingOnMouse.gameObject);
            
        }
    }
}
