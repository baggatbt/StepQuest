using System.Collections;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public interface ILocationProvider
{
    double Latitude { get; }
    double Longitude { get; }
    double Accuracy { get; }  // meters (approx)
}

public class GPSService : MonoBehaviour
{
    [Header("Editor Testing")]
    public bool useFakeInEditor = true;
    public double editorStartLat = 40.712776;  // NYC
    public double editorStartLon = -74.005974;
    public float editorMovePerKeyMeter = 5f;

    [Header("Status (read-only)")]
    public bool ready;
    public string status;

    public ILocationProvider Provider => _provider;

    ILocationProvider _provider;

    // ===== Providers =====
    class DeviceProvider : ILocationProvider
    {
        public double Latitude => Input.location.lastData.latitude;
        public double Longitude => Input.location.lastData.longitude;
        public double Accuracy => Input.location.lastData.horizontalAccuracy;
    }

    class EditorFakeProvider : ILocationProvider
    {
        public double Latitude => _lat;
        public double Longitude => _lon;
        public double Accuracy => 5;

        static double _lat, _lon;

        public static void Init(double lat, double lon) { _lat = lat; _lon = lon; }

        public static void Nudge(Vector2 meters)
        {
            const double mPerDegLat = 111132.0;
            const double mPerDegLonAtMid = 85390.0;
            _lat += meters.y / mPerDegLat;
            _lon += meters.x / mPerDegLonAtMid;
        }
    }

    // === Helpers for fake mode ===
    public bool IsUsingEditorFake
    {
        get
        {
#if UNITY_EDITOR
            return ready && (_provider is EditorFakeProvider);
#else
            return false;
#endif
        }
    }

    public void NudgeEditorMeters(Vector2 meters)
    {
#if UNITY_EDITOR
        if (_provider is EditorFakeProvider) EditorFakeProvider.Nudge(meters);
#endif
    }

    public void SetEditorFakeLatLon(double lat, double lon)
    {
#if UNITY_EDITOR
        if (_provider is EditorFakeProvider) EditorFakeProvider.Init(lat, lon);
#endif
    }

    public double CurrentLatitude => Provider != null ? Provider.Latitude : 0.0;
    public double CurrentLongitude => Provider != null ? Provider.Longitude : 0.0;
    public double CurrentAccuracy => Provider != null ? Provider.Accuracy : 0.0;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        if (useFakeInEditor)
        {
            EditorFakeProvider.Init(editorStartLat, editorStartLon);
            _provider = new EditorFakeProvider();
            status = "EDITOR FAKE: ready";
            ready = true;
            yield break;
        }
#endif

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            float t = 5f;
            while (t > 0f && !Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            { t -= Time.unscaledDeltaTime; yield return null; }
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            status = "PERMISSION DENIED";
            ready = false; yield break;
        }
#endif
        if (!Input.location.isEnabledByUser)
        {
            status = "GPS DISABLED BY USER";
            ready = false; yield break;
        }

        Input.location.Start(5f, 1f);

        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait-- > 0)
        {
            status = "INITIALIZING...";
            yield return new WaitForSeconds(1f);
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            status = "FAILED: " + Input.location.status;
            ready = false; yield break;
        }

        _provider = new DeviceProvider();
        ready = true;
        status = "RUNNING";

        while (true) { yield return null; }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (ready && _provider is EditorFakeProvider)
        {
            Vector2 nudge = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) nudge.y += editorMovePerKeyMeter * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) nudge.y -= editorMovePerKeyMeter * Time.deltaTime;
            if (Input.GetKey(KeyCode.D)) nudge.x += editorMovePerKeyMeter * Time.deltaTime;
            if (Input.GetKey(KeyCode.A)) nudge.x -= editorMovePerKeyMeter * Time.deltaTime;
            if (nudge != Vector2.zero) EditorFakeProvider.Nudge(nudge);
        }
    }
#endif
}
