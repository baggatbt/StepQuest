using UnityEngine;

public static class GeoUtils
{
    // Simple “flat earth” meters per degree around your chosen origin.
    public static void GetMetersPerDegree(double originLatDeg, out double metersPerDegLat, out double metersPerDegLon)
    {
        // WGS84 approximations near a latitude
        double lat = originLatDeg * Mathf.Deg2Rad;
        metersPerDegLat = 111132.92 - 559.82 * Mathf.Cos(2f * (float)lat) + 1.175 * Mathf.Cos(4f * (float)lat);
        metersPerDegLon = 111412.84 * Mathf.Cos((float)lat) - 93.5 * Mathf.Cos(3f * (float)lat);
    }

    // Convert lat/lon → local XY meters relative to origin
    public static Vector2 LatLonToLocalMeters(double latDeg, double lonDeg, double originLatDeg, double originLonDeg)
    {
        GetMetersPerDegree(originLatDeg, out double mLat, out double mLon);
        double dLat = latDeg - originLatDeg;
        double dLon = lonDeg - originLonDeg;
        return new Vector2((float)(dLon * mLon), (float)(dLat * mLat));
    }
}
