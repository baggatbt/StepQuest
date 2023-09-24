using UnityEngine;

public class StepCounterController : MonoBehaviour
{
    private AndroidJavaObject stepCounterPluginInstance;
     private int steps;

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
    int currentSteps = GetStepsSinceStart();
    Debug.Log("Current steps: " + currentSteps);
}



    public int GetStepCount()
    {
        return steps;
    }

    private void OnDestroy()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            stepCounterPluginInstance.CallStatic("stopCounting");
        }
        Debug.Log("On destroy call in step counter ");
    }

   public int GetStepsSinceStart()
{
    Debug.Log("Getting steps since start in controller");
    steps = CallStaticMethodOnPlugin<int>("getStepsSinceStart");
    Debug.Log(steps);
    return steps;
}


    private int GetStepsFromPlugin()
    {
        // Assuming the method CallStaticMethodOnPlugin<int>("getSteps") fetches the latest step count from your plugin
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
