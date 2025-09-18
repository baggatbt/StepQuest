// GPSDebugPanel.cs (drop on a TMP text in your HUD canvas)
using UnityEngine;
using TMPro;

public class GPSDebugPanel : MonoBehaviour
{
    public GPSService gps;
    public GPSController2D map;
    public TextMeshProUGUI label;

    void Update()
    {
        if (!label) return;
        if (gps == null) { label.text = "GPS: (no service)"; return; }
        if (!gps.ready) { label.text = $"GPS: {gps.status}"; return; }

        double lat = gps.Provider.Latitude;
        double lon = gps.Provider.Longitude;
        double acc = gps.Provider.Accuracy;

        var meters = (map != null)
            ? GeoUtils.LatLonToLocalMeters(lat, lon, map.originLatitude, map.originLongitude)
            : Vector2.zero;

        label.text =
            $"GPS: {gps.status}\n" +
            $"Lat: {lat:F6}\nLon: {lon:F6}\nAcc: ±{acc:F0} m\n" +
            $"Local: ({meters.x:F1}, {meters.y:F1})";
    }
}
