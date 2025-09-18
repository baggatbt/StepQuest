using UnityEngine;

public class StepTravelController : MonoBehaviour
{
    [Header("Wiring")]
    public GPSService gps;
    public GPSController2D map; // for calibration info (origin + unitsPerMeter)

    [Header("Economy")]
    public float metersPerStep = 2.0f; // cost: 1 step per 2 meters by default

    void Reset()
    {
        gps = FindObjectOfType<GPSService>();
        map = FindObjectOfType<GPSController2D>();
    }

    public int GetStepCostToNode(MapNode2D node)
    {
        if (gps == null || map == null || !gps.ready) return int.MaxValue;

        var playerLat = gps.Provider.Latitude;
        var playerLon = gps.Provider.Longitude;

        // distance in meters using local flat projection near origin
        var a = GeoUtils.LatLonToLocalMeters(playerLat, playerLon, map.originLatitude, map.originLongitude);
        var b = GeoUtils.LatLonToLocalMeters(node.latitude, node.longitude, map.originLatitude, map.originLongitude);
        float distMeters = Vector2.Distance(a, b);

        return Mathf.CeilToInt(distMeters / Mathf.Max(0.0001f, metersPerStep));
    }

    public bool TravelToNode(MapNode2D node)
    {
        int cost = GetStepCostToNode(node);
        // Uses your existing PlayerData.UseSteps(int)
        if (PlayerData.Instance.UseSteps(cost))  // <-- your project already has this
        {
            // Snap fake GPS to that node (works in Editor); on device, we just move the icon directly:
            if (gps.FakeProvider != null) // Editor fake mode
            {
                gps.FakeProvider.Nudge(node.latitude - gps.FakeProvider.Latitude,
                                        node.longitude - gps.FakeProvider.Longitude);
            }
            else
            {
                // Optional: directly warp the icon (visual only)
                var meters = GeoUtils.LatLonToLocalMeters(node.latitude, node.longitude, map.originLatitude, map.originLongitude);
                var world = new Vector3(meters.x * map.unitsPerMeter, meters.y * map.unitsPerMeter, 0f);
                map.transform.position = world; // or map.playerIcon.position = world;
            }
            Debug.Log($"Traveled to {node.nodeID} using {cost} steps.");
            return true;
        }
        Debug.Log("Not enough steps to travel.");
        return false;
    }
}
