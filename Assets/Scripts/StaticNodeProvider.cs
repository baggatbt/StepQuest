using System.Collections.Generic;
using UnityEngine;

public interface INodeProvider
{
    IEnumerable<NodeData> GetNodesAround(double lat, double lon, float radiusMeters);
}

public class StaticNodeProvider : MonoBehaviour, INodeProvider
{
    public NodeSet nodeSet;               // Option A: ScriptableObject
    public TextAsset nodesJson;           // Option B: JSON (Resources/StreamingAssets)
    List<NodeData> cache;

    void Awake()
    {
        if (nodeSet) cache = new List<NodeData>(nodeSet.nodes);
        else if (nodesJson)
            cache = JsonHelper.FromJsonArray<NodeData>(nodesJson.text);
        else
            cache = new List<NodeData>();
    }

    public IEnumerable<NodeData> GetNodesAround(double lat, double lon, float radiusMeters)
    {
        var result = new List<NodeData>();
        foreach (var nd in cache)
        {
            float d = HaversineMeters(lat, lon, nd.latitude, nd.longitude);
            if (d <= radiusMeters) result.Add(nd);
        }
        return result;
    }

    static float HaversineMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000.0; // Earth radius in meters (double for accuracy)

        double dLat = Mathf.Deg2Rad * (lat2 - lat1);
        double dLon = Mathf.Deg2Rad * (lon2 - lon1);

        double a = System.Math.Sin(dLat / 2) * System.Math.Sin(dLat / 2) +
                   System.Math.Cos(lat1 * Mathf.Deg2Rad) * System.Math.Cos(lat2 * Mathf.Deg2Rad) *
                   System.Math.Sin(dLon / 2) * System.Math.Sin(dLon / 2);

        double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));

        // Explicit cast here fixes CS0266
        return (float)(R * c);
    }
}

// tiny JSON helper (Unity can’t deserialize top-level arrays without a wrapper)
public static class JsonHelper
{
    [System.Serializable] private class Wrapper<T> { public T[] array; }
    public static List<T> FromJsonArray<T>(string json) =>
        new List<T>(JsonUtility.FromJson<Wrapper<T>>("{\"array\":" + json + "}").array);
}
