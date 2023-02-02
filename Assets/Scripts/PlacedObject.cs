using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    private PlacedObjectTypeSO placedObjectTypeSO;
    public Vector2Int origin;
    public  PlacedObjectTypeSO.Dir dir;


    public static PlacedObject Place(Transform placedTransform,Vector2Int origin,PlacedObjectTypeSO.Dir dir,PlacedObjectTypeSO placedObjectTypeSO)
    {
        PlacedObject placedObject=placedTransform.GetComponent<PlacedObject>();
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;

        return placedObject;

    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }


    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
