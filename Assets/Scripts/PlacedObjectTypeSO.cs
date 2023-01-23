using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public int width;
    public int height;
}
