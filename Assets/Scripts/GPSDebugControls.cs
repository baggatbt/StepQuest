using UnityEngine;

public class GPSDebugControls : MonoBehaviour
{
    public GPSService gps;
    [Tooltip("Degrees per key press in Editor fake mode")]
    public double degStep = 0.00005; // ~5-6 meters at mid-lats

    void Reset() { gps = FindObjectOfType<GPSService>(); }

    void Update()
    {
#if UNITY_EDITOR
        if (gps == null || gps.FakeProvider == null) return;
        double dLat = 0, dLon = 0;
        if (Input.GetKey(KeyCode.W)) dLat += degStep;
        if (Input.GetKey(KeyCode.S)) dLat -= degStep;
        if (Input.GetKey(KeyCode.D)) dLon += degStep;
        if (Input.GetKey(KeyCode.A)) dLon -= degStep;
        if (dLat != 0 || dLon != 0) gps.FakeProvider.Nudge(dLat, dLon);
#endif
    }
}
