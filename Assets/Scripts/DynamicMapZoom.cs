using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicMapZoom : MonoBehaviour
{
    [Header("Wiring")]
    public GPSController2D map;                 // for unitsPerMeter
    public NodeSpawner2D spawner;               // optional: to auto-fit the spawn radius

    [Header("Zoom (choose ONE mode)")]
    [Tooltip("If > 0, camera will show approximately this many METERS vertically.")]
    public float verticalMetersOnScreen = 200f;

    [Tooltip("If true, auto-zoom so the full load radius from NodeSpawner2D fits on screen.")]
    public bool fitToSpawnerRadius = false;
    [Range(0.8f, 2.0f)] public float fitPadding = 1.1f;

    [Header("Clamp")]
    public float minOrthoSize = 4f;
    public float maxOrthoSize = 150f;

    Camera cam;

    void Reset()
    {
        cam = GetComponent<Camera>();
        if (!map) map = FindObjectOfType<GPSController2D>();
        if (!spawner) spawner = FindObjectOfType<NodeSpawner2D>();
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void LateUpdate()
    {
        if (map == null) return;

        float unitsPerMeter = Mathf.Max(0.0001f, map.unitsPerMeter);
        float targetOrtho = cam.orthographicSize;

        if (fitToSpawnerRadius && spawner != null)
        {
            // Show full spawn radius vertically, with padding
            float meters = spawner.loadRadiusMeters * 2f * fitPadding; // diameter
            targetOrtho = (meters * unitsPerMeter) * 0.5f;
        }
        else if (verticalMetersOnScreen > 0f)
        {
            targetOrtho = (verticalMetersOnScreen * unitsPerMeter) * 0.5f;
        }

        cam.orthographicSize = Mathf.Clamp(targetOrtho, minOrthoSize, maxOrthoSize);
    }
}
