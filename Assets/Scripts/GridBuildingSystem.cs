using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    private Pathfinding pathfinding;
    private Grid<PathNode> grid;
    private PlacedObjectTypeSO.Dir dir=PlacedObjectTypeSO.Dir.Down;
    private Transform buildingOnMouse;
    private bool isBuilding=false;
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
        Vector3 mouseWorldPosition = UtilTool.GetMouseWorldPosition();
        grid.GetXY(mouseWorldPosition,out int x,out int y);
        PathNode mouseNode = grid.GetGridObject(x, y);
        if ( mouseNode== null) return;

        if (Input.GetMouseButtonDown(1))
        {
            PlacedObject placedObject = mouseNode.GetPlacedObject();
            if (placedObject != null)
            {
                placedObject.DestroySelf();

                List<Vector2Int> DeletedPlacedObject = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in DeletedPlacedObject)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetIsWalkable(true);
                    pathfinding.UpdateRegionNeighbour(grid.GetGridObject(gridPosition.x, gridPosition.y));
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isBuilding = !isBuilding;
        }

        if (!isBuilding)
        {
            if (buildingOnMouse!=null)
            {
                Destroy(buildingOnMouse.gameObject);
            }
            return;
        }

        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
        Vector3 mouseNodeposition =grid.GetWorldPosition(x, y)+new Vector3(rotationOffset.x,rotationOffset.y,0)*grid.GetCellSize();
        if (buildingOnMouse == null)
        {
            buildingOnMouse = Instantiate(placedObjectTypeSO.prefab, mouseNodeposition, Quaternion.Euler(0,0, placedObjectTypeSO.GetRotationAngle(dir)));
            
        }
        Color color_MouseOnBuilding = buildingOnMouse.GetChild(0).GetComponent<SpriteRenderer>().color;
        bool canBuild = true;
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, y), dir);

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (grid.GetGridObject(gridPosition.x, gridPosition.y)==null||!grid.GetGridObject(gridPosition.x,gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        if (canBuild&&color_MouseOnBuilding.a!=0.247f)
        {
            color_MouseOnBuilding.a=0.247f;
            buildingOnMouse.GetChild(0).GetComponent<SpriteRenderer>().color = color_MouseOnBuilding;
        }

        if (!canBuild&&color_MouseOnBuilding.a!=0.5f)
        {
            color_MouseOnBuilding.a = 0.5f;
            buildingOnMouse.GetChild(0).GetComponent<SpriteRenderer>().color = color_MouseOnBuilding;
        }
        buildingOnMouse.position = mouseNodeposition;
        buildingOnMouse.rotation = Quaternion.Euler(0,0, placedObjectTypeSO.GetRotationAngle(dir));
        if (Input.GetMouseButtonDown(0))
        {
            if (canBuild)
            {
                color_MouseOnBuilding.a = 1;
                buildingOnMouse.GetChild(0).GetComponent<SpriteRenderer>().color = color_MouseOnBuilding;
                PlacedObject placedObject=PlacedObject.Place(buildingOnMouse,new Vector2Int(x,y),dir,placedObjectTypeSO);
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetIsWalkable(false);
                    pathfinding.UpdateRegionNeighbour(grid.GetGridObject(gridPosition.x, gridPosition.y));
                }

                buildingOnMouse = null;
                isBuilding = false;
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
