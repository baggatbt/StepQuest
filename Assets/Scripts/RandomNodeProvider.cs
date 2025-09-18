using System.Collections.Generic;
using UnityEngine;

public class RandomNodeProvider : MonoBehaviour, INodeProvider
{
    [Header("Generation")]
    public int count = 30;
    public float radiusMeters = 300;

    System.Random rng;
    List<NodeData> cache;

    void Awake()
    {
        rng = new System.Random(12345);
        cache = new List<NodeData>(count);
    }

    public IEnumerable<NodeData> GetNodesAround(double lat, double lon, float radius)
    {
        if (cache.Count == 0)
        {
            for (int i = 0; i < count; i++)
            {
                var off = RandomOffsetMeters(radiusMeters);
                var (rlat, rlon) = AddMeterOffset(lat, lon, off.x, off.y);

                cache.Add(new NodeData
                {
                    nodeID = "rnd_" + i,
                    kind = NodeKind.Battle,
                    latitude = rlat,
                    longitude = rlon,
                    interactRadiusMeters = 25,
                    battleSceneName = "BattleScene",
                    poolName = "Forest",
                    enemiesToSpawn = 1 + (i % 3),
                    enemyLevel = 1 + (i % 2)
                });
            }
        }
        return cache;
    }

    Vector2 RandomOffsetMeters(float r)
    {
        // disc sample
        float t = (float)(2 * Mathf.PI * rng.NextDouble());
        float u = (float)rng.NextDouble() + (float)rng.NextDouble();
        float d = (u > 1 ? 2 - u : u) * r;
        return new Vector2(Mathf.Cos(t) * d, Mathf.Sin(t) * d);
    }

    static (double lat, double lon) AddMeterOffset(double lat, double lon, float dx, float dy)
    {
        // dx = east meters, dy = north meters
        double mPerDegLat, mPerDegLon;
        GeoUtils.GetMetersPerDegree(lat, out mPerDegLat, out mPerDegLon);
        return (lat + dy / mPerDegLat, lon + dx / mPerDegLon);
    }
}
