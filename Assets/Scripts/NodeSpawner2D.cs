using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeSpawner2D : MonoBehaviour
{
    [Header("Wiring")]
    [Tooltip("Component that implements INodeProvider (e.g., ProceduralNodeProvider).")]
    public MonoBehaviour providerBehaviour; // must implement INodeProvider
    public GPSService gps;
    public GPSController2D map;
    public Transform mapRoot;

    [Header("Prefabs")]
    public GameObject battleNodePrefab;
    public GameObject genericNodePrefab;

    [Header("Streaming / Perf")]
    public float loadRadiusMeters = 600f;
    public float refreshMeters = 80f;
    public float refreshSeconds = 2f;
    public int maxLiveNodes = 300;

    INodeProvider _provider;
    readonly Dictionary<string, GameObject> _live = new();
    Vector2 _lastPlayerMeters;
    float _lastRefreshTime;
    bool _didInitial;

    void Reset()
    {
        if (!gps) gps = FindObjectOfType<GPSService>();
        if (!map) map = FindObjectOfType<GPSController2D>();
        if (!mapRoot && map) mapRoot = map.mapRoot;
        if (!providerBehaviour) providerBehaviour = GetComponent<MonoBehaviour>(); // will validate in Awake
    }

    void Awake()
    {
        // Accept ANY INodeProvider on this GO
        if (providerBehaviour != null && providerBehaviour is INodeProvider p) _provider = p;
        if (_provider == null)
        {
            _provider = GetComponent<INodeProvider>();
            providerBehaviour = _provider as MonoBehaviour;
        }
        if (_provider == null)
        {
            Debug.LogWarning($"{name}: No INodeProvider found/assigned.");
        }
        if (!mapRoot && map) mapRoot = map.mapRoot;
    }

    void Start()
    {
        TryRefresh(force: true);
    }

    void Update()
    {
        TryRefresh(force: false);
    }

    void TryRefresh(bool force)
    {
        if (_provider == null || gps == null || map == null) return;
        if (!gps.ready) return;

        double plat = gps.Provider.Latitude;
        double plon = gps.Provider.Longitude;

        Vector2 playerMeters = GeoUtils.LatLonToLocalMeters(plat, plon, map.originLatitude, map.originLongitude);

        bool movedFar = Vector2.Distance(playerMeters, _lastPlayerMeters) >= refreshMeters;
        bool timeElapsed = (Time.time - _lastRefreshTime) >= refreshSeconds;
        if (!force && !movedFar && !timeElapsed) return;

        _lastPlayerMeters = playerMeters;
        _lastRefreshTime = Time.time;

        var nodes = _provider.GetNodesAround(plat, plon, loadRadiusMeters);
        int before = _live.Count;

        int fetched = nodes is ICollection<NodeData> col ? col.Count : nodes.Count(); // force enumeration once
        Debug.Log($"{name}: Provider={_provider.GetType().Name} fetched={fetched} center=({plat:F6},{plon:F6}) r={loadRadiusMeters}");

        SpawnOrUpdate(nodes);
        ReprojectLiveNodes();
        CullFarNodes(playerMeters, loadRadiusMeters * 1.5f);

        if (!_didInitial)
        {
            _didInitial = true;
            Debug.Log($"{name}: Initial spawn done. Live nodes: {_live.Count}");
        }
        else
        {
            int delta = _live.Count - before;
            Debug.Log($"{name}: Refresh Δ={delta}, Live={_live.Count}");
        }
    }

    void SpawnOrUpdate(IEnumerable<NodeData> nodes)
    {
        foreach (var nd in nodes)
        {
            if (_live.ContainsKey(nd.nodeID)) continue;
            if (_live.Count >= maxLiveNodes)
            {
                Debug.LogWarning($"{name}: Max live nodes reached ({maxLiveNodes}).");
                break;
            }

            GameObject prefab =
                nd.kind == NodeKind.Battle ? battleNodePrefab :
                genericNodePrefab != null ? genericNodePrefab :
                battleNodePrefab;

            if (!prefab)
            {
                Debug.LogWarning($"{name}: Missing prefab for {nd.kind} (id {nd.nodeID}).");
                continue;
            }

            var go = Instantiate(prefab, mapRoot ? mapRoot : transform);
            go.name = nd.nodeID;

            var baseNode = go.GetComponent<MapNode2D>() ?? go.AddComponent<MapNode2D>();
            baseNode.nodeID = nd.nodeID;
            baseNode.kind = nd.kind;
            baseNode.latitude = nd.latitude;
            baseNode.longitude = nd.longitude;
            baseNode.interactRadiusMeters = nd.interactRadiusMeters;

            if (nd.kind == NodeKind.Battle)
            {
                var bm = go.GetComponent<BattleMapNode2D>() ?? go.AddComponent<BattleMapNode2D>();
                bm.battleSceneName = nd.battleSceneName;
                bm.poolName = nd.poolName;
                bm.enemiesToSpawn = Mathf.Max(1, nd.enemiesToSpawn);
                bm.enemyLevel = Mathf.Max(1, nd.enemyLevel);
            }

            PlaceNode(go.transform, nd.latitude, nd.longitude);
            _live[nd.nodeID] = go;
        }
    }

    void PlaceNode(Transform t, double lat, double lon)
    {
        Vector2 meters = GeoUtils.LatLonToLocalMeters(lat, lon, map.originLatitude, map.originLongitude);
        t.localPosition = new Vector3(meters.x * map.unitsPerMeter, meters.y * map.unitsPerMeter, 0f);
    }

    void ReprojectLiveNodes()
    {
        foreach (var go in _live.Values)
        {
            if (!go) continue;
            var n = go.GetComponent<MapNode2D>();
            if (!n) continue;
            PlaceNode(go.transform, n.latitude, n.longitude);
        }
    }

    void CullFarNodes(Vector2 playerMeters, float maxDistMeters)
    {
        var remove = new List<string>();
        foreach (var kv in _live)
        {
            var go = kv.Value;
            if (!go) { remove.Add(kv.Key); continue; }
            Vector3 lp = go.transform.localPosition;
            Vector2 nodeMeters = new(lp.x / map.unitsPerMeter, lp.y / map.unitsPerMeter);
            if (Vector2.Distance(nodeMeters, playerMeters) > maxDistMeters) remove.Add(kv.Key);
        }
        foreach (var id in remove)
        {
            if (_live.TryGetValue(id, out var go) && go) Destroy(go);
            _live.Remove(id);
        }
    }

    // One-click sanity in Inspector
    [ContextMenu("Spawn Test Node At Player")]
    void SpawnTestNodeAtPlayer()
    {
        if (gps == null || map == null || !gps.ready || battleNodePrefab == null)
        { Debug.LogWarning("Missing refs (gps/map/prefab)."); return; }

        double plat = gps.Provider.Latitude;
        double plon = gps.Provider.Longitude;

        var go = Instantiate(battleNodePrefab, mapRoot ? mapRoot : transform);
        go.name = "TestNode_Here";

        var bm = go.GetComponent<BattleMapNode2D>() ?? go.AddComponent<BattleMapNode2D>();
        bm.nodeID = go.name;
        bm.kind = NodeKind.Battle;
        bm.latitude = plat;
        bm.longitude = plon;
        bm.interactRadiusMeters = 25;
        bm.battleSceneName = "BattleScene";
        bm.poolName = "Forest";
        bm.enemiesToSpawn = 2;
        bm.enemyLevel = 1;

        PlaceNode(go.transform, plat, plon);
        _live[go.name] = go;

        Debug.Log("[NodeSpawner2D] Spawned test node at player.");
    }
}
