using UnityEngine;

public class PermissionsRequester : MonoBehaviour
{
    private const string ActivityRecognitionPermission = "android.permission.ACTIVITY_RECOGNITION";
    private const string ReadExternalStoragePermission = "android.permission.READ_EXTERNAL_STORAGE";
    private const string WriteExternalStoragePermission = "android.permission.WRITE_EXTERNAL_STORAGE";

    void Start()
    {
        RequestPermissions();
    }

    void RequestPermissions()
    {
        Debug.Log("Permissions being requested");
        if (Application.platform == RuntimePlatform.Android)
        {
            // Request activity recognition permission
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(ActivityRecognitionPermission))
            {
                UnityEngine.Android.Permission.RequestUserPermission(ActivityRecognitionPermission);
                Debug.Log("Requesting Activity Recognition Permission");
            }

            // Request read external storage permission
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(ReadExternalStoragePermission))
            {
                UnityEngine.Android.Permission.RequestUserPermission(ReadExternalStoragePermission);
                Debug.Log("Requesting Read External Storage Permission");
            }

            // Request write external storage permission
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(WriteExternalStoragePermission))
            {
                UnityEngine.Android.Permission.RequestUserPermission(WriteExternalStoragePermission);
                Debug.Log("Requesting Write External Storage Permission");
            }
        }
    }
}
