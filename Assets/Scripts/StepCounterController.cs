using UnityEngine;

public class StepCounterController : MonoBehaviour
{
    public static StepCounterController Instance { get; private set; }

    private AndroidJavaObject stepCounterPluginInstance;

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
        stepCounterPluginInstance.CallStatic("startCounting");
    }

    private void StopCounting()
    {
        stepCounterPluginInstance.CallStatic("stopCounting");
    }

    public int GetStepsSinceStart() // Steps since the app started
    {
        return CallStaticMethodOnPlugin<int>("getStepsSinceStart");
    }

    public int GetTotalSteps() // Gets the total step count from the device
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
