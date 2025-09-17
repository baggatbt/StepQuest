using UnityEngine;

public class GPSController2D : MonoBehaviour
{
    [Header("Wiring")]
    public Transform playerIcon;          // sprite/transform on your pixel map
    public GPSService gps;                // assign GPSService (same scene or DontDestroy)

    [Header("Map Calibration")]
    public double originLatitude = 40.7128;
    public double originLongitude = -74.0060;
    [Tooltip("How many Unity units equal one meter on your map?")]
    public float unitsPerMeter = 0.1f;    // 10 meters = 1 unit → tweak to fit your pixel scale

    [Header("Smoothing")]
    public float lerpSpeed = 5f;

    Vector3 targetPos;

    void Reset()
    {
        gps = FindObjectOfType<GPSService>();
    }

    void Update()
    {
        if (gps == null || !gps.ready || playerIcon == null) return;

        var lat = gps.Provider.Latitude;
        var lon = gps.Provider.Longitude;

        Vector2 meters = GeoUtils.LatLonToLocalMeters(lat, lon, originLatitude, originLongitude);
        var world = new Vector3(meters.x * unitsPerMeter, meters.y * unitsPerMeter, 0f);
        targetPos = world;

        playerIcon.position = Vector3.Lerp(playerIcon.position, targetPos, Time.deltaTime * lerpSpeed);
    }
}
