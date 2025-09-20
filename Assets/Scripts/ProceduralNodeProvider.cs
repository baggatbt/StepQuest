using System.Collections.Generic;
using UnityEngine;

public class ProceduralNodeProvider : MonoBehaviour, INodeProvider
{
    [Header("Generation")]
    [Tooltip("How many nodes to try to place per query (successful may be fewer due to spacing).")]
    public int targetCount = 40;

    [Tooltip("Minimum spacing between nodes (meters).")]
    public float minSpacingMeters = 25f;

    [Tooltip("Optional: clamp maximum nodes actually returned.")]
    public int maxReturn = 60;

    [Tooltip("Use a stable seed per 'area' so results are consistent while you stay nearby.")]
    public int baseSeed = 12345;

    public int NodeCount => -1; // procedural/infinite

    public IEnumerable<NodeData> GetNodesAround(double centerLat, double centerLon, float radiusMeters)
    {
        var rng = new System.Random(SeedForArea(centerLat, centerLon));
        var placed = new List<Vector2>(targetCount);
        int attempts = 0;
        int maxAttempts = targetCount * 20; // generous retry budget

        while (placed.Count < targetCount && attempts++ < maxAttempts)
        {
            // sample uniform in circle
            float r = Mathf.Sqrt((float)rng.NextDouble()) * radiusMeters;
            float t = (float)(rng.NextDouble() * Mathf.PI * 2f);
            Vector2 candidate = new Vector2(Mathf.Cos(t) * r, Mathf.Sin(t) * r);

            // enforce min spacing
            bool ok = true;
            for (int i = 0; i < placed.Count; i++)
            {
                if (Vector2.SqrMagnitude(candidate - placed[i]) < (minSpacingMeters * minSpacingMeters))
                {
                    ok = false; break;
                }
            }
            if (!ok) continue;

            placed.Add(candidate);
        }

        // Convert to lat/lon and build NodeData
        var results = new List<NodeData>(Mathf.Min(maxReturn, placed.Count));
        int idx = 0;
        foreach (var m in placed)
        {
            Vector2 ll = GeoUtils.LocalMetersToLatLon(m, centerLat, centerLon);

            results.Add(new NodeData
            {
                nodeID = $"proc_{(int)(ll.x * 1e6)}_{(int)(ll.y * 1e6)}_{idx++}", // quantized+idx → unique-ish
                kind = NodeKind.Battle,
                latitude = ll.x,
                longitude = ll.y,
                interactRadiusMeters = 25f,

                // quick battle fields
                battleSceneName = "TestPortraitBattle",
                poolName = "Forest",
                enemiesToSpawn = 1 + (idx % 2),
                enemyLevel = 1 + (idx % 2)
            });

            if (results.Count >= maxReturn) break;
        }

        return results;
    }

    int SeedForArea(double lat, double lon)
    {
        // Quantize center to ~100m grid so the set is stable while you’re nearby,
        // but changes once you move noticeably (prevents total reshuffle every small step).
        GeoUtils.GetMetersPerDegree(lat, out double mLat, out double mLon);
        int qx = Mathf.RoundToInt((float)((lon) * mLon / 100.0));
        int qy = Mathf.RoundToInt((float)((lat) * mLat / 100.0));
        unchecked
        {
            int h = baseSeed;
            h = (h * 16777619) ^ qx;
            h = (h * 16777619) ^ qy;
            return h;
        }
    }
}
