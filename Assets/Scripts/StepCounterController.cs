using UnityEngine;

public class StepCounterController : MonoBehaviour
{
    private AndroidJavaObject stepCounterPluginInstance;
    private int inGameSteps;

    private void Awake()
    {
        Debug.Log("Awake in StepCounterController");
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                using (AndroidJavaClass stepCounterPluginClass = new AndroidJavaClass("com.example.stepcounterplugin.StepCounterPlugin"))
                {
                    stepCounterPluginClass.CallStatic("init", currentActivity);
                    Debug.Log("Plugin Initialized");
                }
            }
            
            stepCounterPluginInstance = new AndroidJavaObject("com.example.stepcounterplugin.StepCounterPlugin");
            stepCounterPluginInstance.CallStatic("startCounting");
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Steps since start: " + GetStepsSinceStart());
        }
    }

    
    private void OnDestroy()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            stepCounterPluginInstance.CallStatic("stopCounting");
        }
    }

    public int GetStepsSinceStart()
    {
        return CallStaticMethodOnPlugin<int>("getStepsSinceStart");
    }

    public int GetSteps()
    {
        return CallStaticMethodOnPlugin<int>("getSteps");
    }

    private T CallStaticMethodOnPlugin<T>(string methodName)
    {
        if (Application.platform == RuntimePlatform.Android && stepCounterPluginInstance != null)
        {
            return stepCounterPluginInstance.CallStatic<T>(methodName);
        }
        return default(T);
    }
}
