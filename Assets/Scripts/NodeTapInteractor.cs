using UnityEngine;
using UnityEngine.EventSystems;

public class NodeTapInteractor : MonoBehaviour
{
    public Camera cam;                 // Main Camera
    public GPSController2D map;        // for origin + unitsPerMeter
    public LayerMask nodeMask;         // layer your nodes are on (Default if unsure)
    public float extraInteractMeters = 0f; // add tolerance if you like

    void Reset()
    {
        cam = Camera.main;
        if (!map) map = FindObjectOfType<GPSController2D>();
        nodeMask = ~0; // everything
    }

    void Update()
    {
        // mouse for Editor, touch for device
        if (Input.GetMouseButtonDown(0)) TryTap(Input.mousePosition);
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            TryTap(Input.touches[0].position);
    }

    void TryTap(Vector2 screenPos)
    {
        // Don’t steal taps from UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
#if UNITY_ANDROID || UNITY_IOS
        // On mobile, UI modules sometimes need pointerId
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(0)) return;
#endif

        var ray = cam.ScreenPointToRay(screenPos);
        var hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, nodeMask);
        if (hit.collider == null) return;

        var node = hit.collider.GetComponent<BattleMapNode2D>() ??
                   hit.collider.GetComponentInParent<BattleMapNode2D>();
        if (!node) return;

        // Distance check (meters)
        if (map == null || map.gps == null || !map.gps.ready) return;
        double plat = map.gps.Provider.Latitude;
        double plon = map.gps.Provider.Longitude;

        // compute meters between player and node using your GeoUtils
        Vector2 p = GeoUtils.LatLonToLocalMeters(plat, plon, map.originLatitude, map.originLongitude);
        Vector2 n = GeoUtils.LatLonToLocalMeters(node.latitude, node.longitude, map.originLatitude, map.originLongitude);
        float distMeters = Vector2.Distance(p, n);

        float allow = node.interactRadiusMeters + extraInteractMeters;
        if (distMeters <= allow)
        {
            node.EnterBattle();
        }
        else
        {
            Debug.Log($"Tapped {node.nodeID}, but out of range ({distMeters:F1}m > {allow}m)");
            // Optional: select + show a HUD prompt to travel/teleport-steps, etc.
        }
    }
}
