using UnityEngine;

public class StepCounterController : MonoBehaviour
{
    private AndroidJavaObject stepCounterPluginInstance;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // Access the Unity Player Activity
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                // Initialize the step counter plugin
                using (AndroidJavaClass stepCounterPluginClass = new AndroidJavaClass("com.example.stepcounterplugin.StepCounterPlugin"))
                {
                    stepCounterPluginClass.CallStatic("init", currentActivity);
                    Debug.Log("Plugin Initialized");
                }
            }
            
            // Create an instance of the plugin class
            stepCounterPluginInstance = new AndroidJavaObject("com.example.stepcounterplugin.StepCounterPlugin");

            // Start counting steps
            stepCounterPluginInstance.CallStatic("startCounting");
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // Optionally print the steps since starting the app to the Unity console
            Debug.Log("Steps since start: " + GetStepsSinceStart());
        }
    }

    private void OnDestroy()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // Stop counting steps when the app closes or the object is destroyed
            stepCounterPluginInstance.CallStatic("stopCounting");
        }
    }

    public int GetStepsSinceStart()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return stepCounterPluginInstance.CallStatic<int>("getStepsSinceStart");
        }
        return 0; // return 0 if not on Android or any other default behavior
    }
}
