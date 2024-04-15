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

            Debug.Log("Awake in StepCounterController");
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    
                    using (AndroidJavaClass stepCounterPluginClass = new AndroidJavaClass("com.example.mylibrary.StepCounterPlugin"))
                    {
                        stepCounterPluginClass.CallStatic("init", currentActivity);
                        Debug.Log("Plugin Initialized");
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

     public void SaveLastKnownSteps()
    {
        int currentTotalSteps = GetSteps();
        PlayerPrefs.SetInt("LastKnownSteps", currentTotalSteps);
        PlayerPrefs.Save();
        Debug.Log("Last known steps saved: " + currentTotalSteps);
    }

    public int GetInitialStepsOnResume()
    {
        return PlayerPrefs.GetInt("LastKnownSteps", 0);
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