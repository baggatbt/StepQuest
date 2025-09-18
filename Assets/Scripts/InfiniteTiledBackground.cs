// InfiniteTiledBackground.cs
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class InfiniteTiledBackground : MonoBehaviour
{
    [Header("Wiring")]
    public Camera cam;            // world camera
    public Transform follow;      // playerIcon (or camera)
    public GPSController2D map;   // for unitsPerMeter

    [Header("Tiling")]
    [Tooltip("Meters per one texture repeat (tile size in world meters).")]
    public float metersPerTile = 24f;
    [Tooltip("1 = lock to world. Slightly <1 or >1 adds gentle parallax.")]
    public float parallax = 1f;
    [Tooltip("Extra world units added around edges to avoid popping.")]
    public float viewMarginUnits = 2f;

    Material mat;
    MeshRenderer mr;

    void Reset()
    {
        cam = Camera.main;
        map = FindObjectOfType<GPSController2D>();
    }

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        mat = Instantiate(mr.sharedMaterial);
        mr.sharedMaterial = mat;
        if (mat.mainTexture) mat.mainTexture.wrapMode = TextureWrapMode.Repeat;
    }

    void LateUpdate()
    {
        if (!cam || !map) return;
        float upm = Mathf.Max(0.0001f, map.unitsPerMeter);

        // 1) Resize quad to cover the visible view (plus margin)
        float heightUnits = cam.orthographicSize * 2f;
        float widthUnits = heightUnits * cam.aspect;
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        transform.localScale = new Vector3(widthUnits + viewMarginUnits, heightUnits + viewMarginUnits, 1f);

        // 2) Set tiling so N repeats fit across the quad based on metersPerTile
        float repeatsX = transform.localScale.x / (metersPerTile * upm);
        float repeatsY = transform.localScale.y / (metersPerTile * upm);
        mat.mainTextureScale = new Vector2(repeatsX, repeatsY);

        // 3) Scroll texture offset from follow (player or camera) to lock to world
        Vector3 w = follow ? follow.position : cam.transform.position;
        float worldMetersX = w.x / upm;
        float worldMetersY = w.y / upm;
        float offX = (worldMetersX / metersPerTile) * parallax;
        float offY = (worldMetersY / metersPerTile) * parallax;
        mat.mainTextureOffset = new Vector2(offX, offY);
    }
}
