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
    private int stepsSinceLastReset; // To keep track of steps since last reset
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
            stepCounterController.SaveLastKnownSteps();
        }
        else
        {
            // Update the step offset based on the current steps count from the device
            UpdateStepOffset();
        }
    }

     private void OnApplicationQuit()
    {
        SavePlayerData();
        stepCounterController.SaveLastKnownSteps();
    }

   private void InitializeStepCounter()
    {
        stepCounterController = FindObjectOfType<StepCounterController>();
        if (stepCounterController == null)
        {
            Debug.LogError("StepCounterController not found in the scene!");
            return;
        }
        stepsSinceLastReset = stepCounterController.GetStepsSinceStart();
    }

    private void HandleFirstLogin()
    {
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstTimeLogin", firstTimeLogin ? 1 : 0);
        SavePlayerData();
    }

    private void UpdateStepCount()
    {
        // Get the current steps from the step counter
        int currentSteps = stepCounterController.GetStepsSinceStart();

        // Calculate the new in-game steps considering the previous steps and offset
        inGameSteps = currentSteps - stepsSinceLastReset;

        // Save the player data if the step count has changed
        if (inGameSteps != PlayerPrefs.GetInt("InGameSteps", 0))
        {
            SavePlayerData();
        }
    }

    private void UpdateStepOffset()
    {
        // Get the last known steps saved before the application was paused or quit
        int lastKnownSteps = stepCounterController.GetInitialStepsOnResume();

        // Update the offset if the current steps are less than the last known steps
        if (lastKnownSteps > stepsSinceLastReset)
        {
            stepsSinceLastReset = lastKnownSteps;
        }
    }

    public void SavePlayerData()
{
    PlayerPrefs.SetInt("PlayerLevel", level);
    PlayerPrefs.SetInt("PlayerGold", gold);
    PlayerPrefs.SetInt("CurrentStageIndex", currentStageIndex);

    // Save the current step count when saving player data
    PlayerPrefs.SetInt("InGameSteps", inGameSteps);
    PlayerPrefs.SetInt("StepsSinceLastReset", stepsSinceStart);

    Debug.Log("Saving Player Data with in-game steps: " + inGameSteps);

    

    PlayerPrefs.Save();
}


    public void LoadPlayerData()
{
    level = PlayerPrefs.GetInt("PlayerLevel", 1);
    gold = PlayerPrefs.GetInt("PlayerGold", 0);
    currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex", 0);
    firstTimeLogin = PlayerPrefs.GetInt("FirstLogin", 0) == 1;

    // Load the saved step count
    inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);

    // Load the step count from the last reset to calculate the current step count
    stepsSinceStart = PlayerPrefs.GetInt("StepsSinceLastReset", stepCounterController.GetStepsSinceStart());
    
    // The actual step count should be updated in the Update method using the current sensor value
    Debug.Log("Loading Player Data with in-game steps: " + inGameSteps);
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
    // Reset the in-game steps count
    inGameSteps = 0;

    // Also reset the steps counted since the last reset
    stepsSinceStart = stepCounterController.GetStepsSinceStart();

    // Immediately save this change to PlayerPrefs
    SavePlayerData();

    Debug.Log("In-game steps have been reset to 0.");
}


}
 