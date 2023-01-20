using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;
using TMPro;
//Custom type Grid
public class GridXZ<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }
    private int width;
    private int height;
    private TGridObject[,] gridArray;
    private Vector3 originPosition;
    private float cellSize;
    private TextMeshPro[,] debugTextArray;
    public GridXZ(int width,int height,float cellSize,Vector3 originPosition,Func<GridXZ<TGridObject>,int,int,TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new TGridObject[width,height];
        debugTextArray = new TextMeshPro[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this,x,z);
            }
        }

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int z = 0; z < gridArray.GetLength(1); z++)
            {
                //debugTextArray[x,z]=UtilsClass.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z)+new Vector3(cellSize,0,cellSize)*.5f,5,Color.white,TextAnchor.MiddleCenter);
                debugTextArray[x, z] = UtilsClassTMP.CreateWorldText(gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) +new Vector3(cellSize,1,cellSize)*.5f, 10, Color.white);

                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1),Color.white,1000f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 1000f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width,height),Color.white,1000f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height),Color.white,1000f);
        
        OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
          {
              debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
          };
    }

    
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x,int z)
    {
        return new Vector3(x,0,z) * cellSize+originPosition;
    }

    public void GetXZ(Vector3 worldPosition,out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition-originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition-originPosition).z / cellSize);
    }

    public void SetGridObject(int x,int z, TGridObject value)
    {
        if(x>=0 && z>=0 && x<width && z < height)
        {
            gridArray[x, z] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
        }
        
    }

    public void TriggerGridObjectChanged(int x,int z)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x,int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];

        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    
}
