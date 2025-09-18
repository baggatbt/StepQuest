using UnityEngine;

public class GPSController2D : MonoBehaviour
{
    public enum OriginMode { Fixed, DynamicAtStart, AlwaysRecenter }

    [Header("Wiring")]
    public Transform playerIcon;     // sprite under your map
    public GPSService gps;           // your GPSService
    public Transform mapRoot;        // parent of your tilemap/world (set this!)

    [Header("Map Calibration")]
    public OriginMode originMode = OriginMode.DynamicAtStart;
    public double originLatitude;
    public double originLongitude;
    [Tooltip("Unity units per meter in your world (e.g., 0.05 = 20m per unit).")]
    public float unitsPerMeter = 0.1f;

    [Header("Smoothing")]
    public float lerpSpeed = 8f;

    bool originInitialized;
    Vector3 targetLocalPos;

    void Reset()
    {
        gps = FindObjectOfType<GPSService>();
        if (mapRoot == null && transform.parent != null) mapRoot = transform.parent;
    }

    void Update()
    {
        if (gps == null || !gps.ready || playerIcon == null) return;

        // Initialize or update origin
        if (!originInitialized || originMode == OriginMode.AlwaysRecenter)
        {
            if (originMode == OriginMode.DynamicAtStart || originMode == OriginMode.AlwaysRecenter)
            {
                originLatitude = gps.Provider.Latitude;
                originLongitude = gps.Provider.Longitude;
                originInitialized = true;
            }
        }

        // Convert GPS → local meters around origin
        var lat = gps.Provider.Latitude;
        var lon = gps.Provider.Longitude;
        Vector2 meters = GeoUtils.LatLonToLocalMeters(lat, lon, originLatitude, originLongitude);

        // Convert meters → local units
        Vector3 local = new Vector3(meters.x * unitsPerMeter, meters.y * unitsPerMeter, 0);

        // Apply as LOCAL position under your Map Root so values stay small (near ~0..±100)
        if (playerIcon.parent != mapRoot) playerIcon.SetParent(mapRoot, worldPositionStays: false);
        targetLocalPos = local;

        // Smooth move
        playerIcon.localPosition = Vector3.Lerp(playerIcon.localPosition, targetLocalPos, Time.deltaTime * lerpSpeed);
    }

    // Call this from a button to align origin to the current GPS and keep the icon near (0,0)
    public void SetOriginToCurrentGPS()
    {
        if (gps == null || !gps.ready) return;
        originLatitude = gps.Provider.Latitude;
        originLongitude = gps.Provider.Longitude;
        originInitialized = true;
    }
}
