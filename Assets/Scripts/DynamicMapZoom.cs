// DynamicMapZoom.cs
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class DynamicMapZoom : MonoBehaviour
{
    public GPSController2D map;
    [Tooltip("How many REAL meters tall the camera shows.")]
    public float verticalMetersOnScreen = 300f;
    public float minOrtho = 3f, maxOrtho = 200f;

    Camera cam;
    void Awake() { cam = GetComponent<Camera>(); cam.orthographic = true; }
    void LateUpdate()
    {
        if (!map) return;
        float upm = Mathf.Max(0.0001f, map.unitsPerMeter);
        float ortho = (verticalMetersOnScreen * upm) * 0.5f;
        cam.orthographicSize = Mathf.Clamp(ortho, minOrtho, maxOrtho);
    }
}
// ZoomHotkeys.cs

public class ZoomHotkeys : MonoBehaviour
{
    public DynamicMapZoom zoom;
    public float step = 50f; // meters per tap/scroll
    void Update()
    {
        if (!zoom) return;
        if (Input.GetKeyDown(KeyCode.Equals) || Input.mouseScrollDelta.y > 0) zoom.verticalMetersOnScreen = Mathf.Max(20, zoom.verticalMetersOnScreen - step);
        if (Input.GetKeyDown(KeyCode.Minus) || Input.mouseScrollDelta.y < 0) zoom.verticalMetersOnScreen += step;
    }
}
