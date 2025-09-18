using UnityEngine;

public static class GeoUtils
{
    // Approx meters per degree near a latitude
    public static void GetMetersPerDegree(double originLatDeg, out double metersPerDegLat, out double metersPerDegLon)
    {
        double lat = originLatDeg * Mathf.Deg2Rad;
        metersPerDegLat = 111132.92 - 559.82 * Mathf.Cos(2f * (float)lat) + 1.175 * Mathf.Cos(4f * (float)lat);
        metersPerDegLon = 111412.84 * Mathf.Cos((float)lat) - 93.5 * Mathf.Cos(3f * (float)lat);
    }

    // lat/lon → local meters (relative to origin)
    public static Vector2 LatLonToLocalMeters(double latDeg, double lonDeg, double originLatDeg, double originLonDeg)
    {
        GetMetersPerDegree(originLatDeg, out double mLat, out double mLon);
        double dLat = latDeg - originLatDeg;
        double dLon = lonDeg - originLonDeg;
        return new Vector2((float)(dLon * mLon), (float)(dLat * mLat));
    }

    // local meters → lat/lon (relative to origin)
    public static Vector2 LocalMetersToLatLon(Vector2 localMeters, double originLatDeg, double originLonDeg)
    {
        GetMetersPerDegree(originLatDeg, out double mLat, out double mLon);
        double lat = originLatDeg + localMeters.y / mLat;
        double lon = originLonDeg + localMeters.x / mLon;
        return new Vector2((float)lat, (float)lon);
    }
}
