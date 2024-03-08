using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RadialMenuLayout : MonoBehaviour
{
    public float radius = 500f; // Distance from the center

    void Start()
    {
        PositionButtons();
    }

    void PositionButtons()
    {
        int children = transform.childCount;
        float angleStep = 360f / children;
        for (int i = 0; i < children; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Vector3 position = CalculatePosition(angleStep * i);
            child.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }

    Vector3 CalculatePosition(float angle)
    {
        // Angle in radians for the Mathf.Cos and Mathf.Sin functions
        float angleRad = angle * (Mathf.PI / 180f);
        float x = Mathf.Cos(angleRad) * radius;
        float y = Mathf.Sin(angleRad) * radius;
        return new Vector3(x, y, 0f);
    }
}
