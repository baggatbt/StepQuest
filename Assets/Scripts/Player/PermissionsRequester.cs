using UnityEngine;

public class PermissionsRequester : MonoBehaviour
{
    private const string ActivityRecognitionPermission = "android.permission.ACTIVITY_RECOGNITION";

    void Start()
    {
        RequestPermission();
    }

    void RequestPermission()
    {
        Debug.Log("Permissions being requested");
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(ActivityRecognitionPermission))
            {
                UnityEngine.Android.Permission.RequestUserPermission(ActivityRecognitionPermission);
            }
        }
    }
}
