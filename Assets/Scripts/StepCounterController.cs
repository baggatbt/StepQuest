using UnityEngine;

public class StepCounterController : MonoBehaviour
{
    public static StepCounterController Instance { get; private set; }

    private AndroidJavaObject stepCounterPluginInstance;

    // Debug variable for non-Android platforms
    private int debugTotalSteps = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    using (AndroidJavaClass stepCounterPluginClass = new AndroidJavaClass("com.example.mylibrary.StepCounterPlugin"))
                    {
                        stepCounterPluginClass.CallStatic("init", currentActivity);
                    }
                }

                stepCounterPluginInstance = new AndroidJavaObject("com.example.mylibrary.StepCounterPlugin");
                StartCounting();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (pauseStatus)
            {
                StopCounting();
            }
            else
            {
                StartCounting();
            }
        }
    }

    private void StartCounting()
    {
        if (Application.platform == RuntimePlatform.Android && stepCounterPluginInstance != null)
        {
            stepCounterPluginInstance.CallStatic("startCounting");
        }
    }

    private void StopCounting()
    {
        if (Application.platform == RuntimePlatform.Android && stepCounterPluginInstance != null)
        {
            stepCounterPluginInstance.CallStatic("stopCounting");
        }
    }

    /// <summary>
    /// Returns total steps from the plugin on Android,
    /// or the debug counter in the Editor / other platforms.
    /// </summary>
    public int GetTotalSteps()
    {
        if (Application.platform == RuntimePlatform.Android && stepCounterPluginInstance != null)
        {
            return CallStaticMethodOnPlugin<int>("getSteps");
        }
        else
        {
            // Use our debug variable in the Editor / other platforms
            return debugTotalSteps;
        }
    }

    public int GetStepsSinceStart()
    {
        if (Application.platform == RuntimePlatform.Android && stepCounterPluginInstance != null)
        {
            return CallStaticMethodOnPlugin<int>("getStepsSinceStart");
        }
        else
        {
            // Not implemented for debug. You can do something similar if needed.
            return 0;
        }
    }

    private T CallStaticMethodOnPlugin<T>(string methodName)
    {
        if (Application.platform == RuntimePlatform.Android && stepCounterPluginInstance != null)
        {
            return stepCounterPluginInstance.CallStatic<T>(methodName);
        }
        return default(T);
    }

    // --------------- DEBUG METHODS ---------------

    // A quick method so we can add steps in the Editor
    public void DebugAddSteps(int stepsToAdd)
    {
        debugTotalSteps += stepsToAdd;
        Debug.Log($"[DEBUG] Added {stepsToAdd} steps. debugTotalSteps = {debugTotalSteps}");
    }

    [ContextMenu("Add 100 Debug Steps")]
    private void Add100DebugStepsMenu()
    {
        DebugAddSteps(100);
    }
}
