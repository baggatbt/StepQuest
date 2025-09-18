using System.Linq;
using UnityEngine;

public class NodeInteractor : MonoBehaviour
{
    [Header("Wiring")]
    public GPSService gps;
    public GPSController2D map;
    public StepTravelController stepTravel;

    [Header("Keys")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode stepTravelKey = KeyCode.T;

    void Reset()
    {
        gps = FindObjectOfType<GPSService>();
        map = FindObjectOfType<GPSController2D>();
        stepTravel = FindObjectOfType<StepTravelController>();
    }

    void Update()
    {
        if (gps == null || map == null || !gps.ready) return;

        var nodes = FindObjectsOfType<MapNode2D>();
        if (nodes.Length == 0) return;

        var pLat = gps.Provider.Latitude;
        var pLon = gps.Provider.Longitude;
        var p = GeoToMeters(pLat, pLon);

        // find nearest
        var nearest = nodes
            .Select(n => new { node = n, dist = Vector2.Distance(p, GeoToMeters(n.latitude, n.longitude)) })
            .OrderBy(x => x.dist)
            .First();

        // E: interact if in radius
        if (Input.GetKeyDown(interactKey))
        {
            if (nearest.dist <= nearest.node.interactRadiusMeters)
            {
                if (nearest.node is BattleMapNode2D bm) bm.EnterBattle();
                else Debug.Log($"Interacted with {nearest.node.nodeID} ({nearest.node.kind})");
            }
            else
            {
                Debug.Log($"Too far ({nearest.dist:0}m). Need ≤ {nearest.node.interactRadiusMeters}m.");
            }
        }

        // T: step travel to nearest node, then auto-battle if it’s a Battle node
        if (Input.GetKeyDown(stepTravelKey) && stepTravel != null)
        {
            if (stepTravel.TravelToNode(nearest.node))
            {
                if (nearest.node is BattleMapNode2D bm) bm.EnterBattle();
            }
        }
    }

    Vector2 GeoToMeters(double lat, double lon)
    {
        return GeoUtils.LatLonToLocalMeters(lat, lon, map.originLatitude, map.originLongitude);
    }
}
