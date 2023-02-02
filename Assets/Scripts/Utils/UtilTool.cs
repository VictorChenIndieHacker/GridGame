using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public static class UtilTool
{
    public const int sortingOrderDefault = 5000;
    // Create Text in the World
    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAlignmentOptions textAlignment = TextAlignmentOptions.Center, int sortingOrder = sortingOrderDefault)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAlignment, sortingOrder);
    }

    // Create Text in the World
    public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAlignmentOptions textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshPro.alignment = textAlignment;
        textMeshPro.text = text;
        textMeshPro.fontSize = fontSize;
        textMeshPro.color = color;
        textMeshPro.sortingOrder = sortingOrder;
        return textMeshPro;
    }

    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        return worldPosition;
    }
    
}
