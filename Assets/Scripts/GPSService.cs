using System.Collections;
using UnityEngine;

public interface IGPSProvider
{
    bool IsRunning { get; }
    double Latitude { get; }
    double Longitude { get; }
}

public class DeviceGPSProvider : IGPSProvider
{
    public bool IsRunning => Input.location.status == LocationServiceStatus.Running;
    public double Latitude => Input.location.lastData.latitude;
    public double Longitude => Input.location.lastData.longitude;
}

public class FakeGPSProvider : IGPSProvider
{
    public bool IsRunning => true;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public FakeGPSProvider(double startLat, double startLon) { Latitude = startLat; Longitude = startLon; }
    public void Nudge(double dLat, double dLon) { Latitude += dLat; Longitude += dLon; }
}

public class GPSService : MonoBehaviour
{
    public static GPSService Instance { get; private set; }

    [Header("Mode")]
    public bool useFakeInEditor = true;
    public double editorStartLat = 40.7128;     // NYC-ish
    public double editorStartLon = -74.0060;

    [Header("Status (read-only)")]
    public bool ready;
    public string status;

    public IGPSProvider Provider { get; private set; }
    public FakeGPSProvider FakeProvider { get; private set; } // exposed for debug

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
#if UNITY_EDITOR
        if (useFakeInEditor)
        {
            FakeProvider = new FakeGPSProvider(editorStartLat, editorStartLon);
            Provider = FakeProvider;
            ready = true;
            status = "Using FAKE GPS (Editor)";
            yield break;
        }
#endif
        Provider = new DeviceGPSProvider();

        if (!Input.location.isEnabledByUser)
        {
            status = "GPS disabled by user";
            yield break;
        }

        Input.location.Start(1f, 0.1f); // desired accuracy, distance filter
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
            yield return new WaitForSeconds(1);

        if (Input.location.status != LocationServiceStatus.Running)
        {
            status = "GPS failed to start";
            yield break;
        }

        status = "GPS running";
        ready = true;
    }
}
