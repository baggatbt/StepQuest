using System;
using UnityEngine;
using System.Collections.Generic;

   

public class PlayerData : MonoBehaviour
{
    private static PlayerData _instance;

    public static PlayerData Instance
    {
        get { return _instance; }
    }

    public int level;
    public int exp;
    public int gold;
    public int attackPower;
    public int defensePower;
    public int inGameSteps;
    private int stepsSinceStart;
    private int previousSteps;
    public int currentSteps;
    public int dailySteps;
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int teamEnergy;
    public string heroID;
    public int speed;
    public int currentStageIndex;
    public bool firstTimeLogin = true;

    public Dictionary<string, int> skillLevels;
    public Dictionary<string, int> skillExp;
    public List<Quest> activeMissions = new List<Quest>();

    private StepCounterController stepCounterController;

    // New fields for tracking steps correctly after reboots
    private int stepsOffset; // The number of steps to offset due to reboots

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeStepCounter();
            LoadPlayerData();
            Debug.Log("PlayerData Awake complete");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (firstTimeLogin)
        {
            HandleFirstLogin();
        }
    }

    private void Update()
    {
        UpdateStepCount();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerData();
        }
        else
        {
            // The game is resuming from the pause, so update steps
            stepsSinceStart = stepCounterController.GetStepsSinceStart();
            UpdateStepOffset();
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    private void InitializeStepCounter()
    {
        stepCounterController = FindObjectOfType<StepCounterController>();
        if (stepCounterController == null)
        {
            Debug.LogError("StepCounterController not found in the scene!");
            return;
        }

        stepsSinceStart = stepCounterController.GetStepsSinceStart();
        previousSteps = PlayerPrefs.GetInt("PreviousSteps", stepsSinceStart);
        stepsOffset = PlayerPrefs.GetInt("StepsOffset", 0);
        inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);
    }

    private void HandleFirstLogin()
    {
        PlayerPrefs.SetInt("FirstLogin", 0);
        firstTimeLogin = false;
        PlayerPrefs.Save();
    }

    private void UpdateStepCount()
    {
        currentSteps = stepCounterController.GetStepsSinceStart();
        if (currentSteps < previousSteps)
        {
            // A reboot likely occurred
            stepsOffset += previousSteps;
        }

        int newSteps = currentSteps + stepsOffset - stepsSinceStart;
        if (newSteps != inGameSteps)
        {
            Debug.Log("New steps comapred to In game steps" + newSteps + inGameSteps);
            inGameSteps = newSteps;
            SavePlayerData();
        }

        previousSteps = currentSteps;
    }

    private void UpdateStepOffset()
    {
        int currentSensorSteps = stepCounterController.GetStepsSinceStart();
        if (currentSensorSteps < stepsSinceStart)
        {
            // The sensor was reset, likely due to a reboot
            stepsOffset = inGameSteps;
        }

        stepsSinceStart = currentSensorSteps;
        PlayerPrefs.SetInt("StepsSinceStart", stepsSinceStart);
        PlayerPrefs.SetInt("PreviousSteps", previousSteps);
        PlayerPrefs.SetInt("StepsOffset", stepsOffset);
        PlayerPrefs.Save();
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("CurrentStageIndex", currentStageIndex);
        PlayerPrefs.SetInt("InGameSteps", inGameSteps);
        Debug.Log("Saving inGameSteps = " + inGameSteps);
        PlayerPrefs.SetInt("PreviousSteps", previousSteps);
        PlayerPrefs.SetInt("StepsOffset", stepsOffset);
        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        level = PlayerPrefs.GetInt("PlayerLevel", 1);
        gold = PlayerPrefs.GetInt("PlayerGold", 0);
        currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex", 0);
        inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);
        Debug.Log("Loading InGameSteps = " + inGameSteps);
        previousSteps = PlayerPrefs.GetInt("PreviousSteps", 0);
        stepsOffset = PlayerPrefs.GetInt("StepsOffset", 0);
        firstTimeLogin = PlayerPrefs.GetInt("FirstLogin", 0) == 1;
    }


    

        public bool UseSteps(int amountToUse)
    {
        if (inGameSteps >= amountToUse)
        {
            inGameSteps -= amountToUse;
            Debug.Log("Spent steps: " + amountToUse);
            return true;
        }
        return false;
    }
    public void ResetSteps()
    {
        inGameSteps = 0;
        SavePlayerData();
        Debug.Log("In-game steps have been reset to 0.");
    }

}
 