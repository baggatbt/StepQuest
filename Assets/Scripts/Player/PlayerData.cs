using System;
using UnityEngine;
using System.Collections.Generic;

// HANDLES STORING/RETRIEVING OF ALL PLAYER RELATED DATA
public class PlayerData : MonoBehaviour
{
    private static PlayerData _instance;

    public static PlayerData Instance
    {
        get { return _instance; }
    }

    // Declare fields for player attributes
    public int level;
    public int exp;
    public int gold;
    public int attackPower;
    public int defensePower;
    public int inGameSteps;
    public int stepsSinceStart; // Initial steps when the game was first loaded
    private int previousSteps; // Declare previousSteps to store the last known step count
    public int currentSteps;
    public int stepsAtCloseOfApp;
    public int dailySteps; // Future implementation for daily step tracking
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int teamEnergy;
    public string heroID;
    public int speed;
    public int currentStageIndex;

    public Dictionary<string, int> skillLevels;
    public Dictionary<string, int> skillExp;
    public List<Quest> activeMissions = new List<Quest>();

    private StepCounterController stepCounterController;

    private void Awake()
    {
        Debug.Log("PlayerData Awake is running");
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (!PlayerPrefs.HasKey("FirstLogin"))
            {
                firstTimeLogin = true;
            }

            LoadPlayerData();
            LoadStepsData(); // Ensure step data is also loaded here
            Debug.Log($"First time login after LoadPlayerData: {firstTimeLogin}");
        }
        else
        {
            Destroy(gameObject);
        }

        skillLevels = new Dictionary<string, int>();
        skillExp = new Dictionary<string, int>();

        stepCounterController = FindObjectOfType<StepCounterController>();
        if (stepCounterController == null)
        {
            Debug.LogError("StepCounterController not found in the scene!");
        }
    }

    private void Start()
    {
        LoadPlayerData(); // Ensure all player data is loaded

        if (firstTimeLogin)
        {
            HandleFirstLogin();
        }
        else
        {
            stepsSinceStart = PlayerPrefs.GetInt("StepsSinceStart", 0); // Load stepsSinceStart from PlayerPrefs
            previousSteps = PlayerPrefs.GetInt("PreviousSteps", 0); // Fetch last known steps
            currentSteps = stepCounterController.GetSteps(); // Fetch current steps
            inGameSteps = PlayerPrefs.GetInt("InGameSteps");
            Debug.Log($"Game restarted: stepsSinceStart: {stepsSinceStart}, currentSteps: {currentSteps}, inGameSteps: {inGameSteps}");
        }
    }

    private void HandleFirstLogin()
    {
        stepsSinceStart = stepCounterController.GetStepsSinceStart(); // Get current step count from sensor
        PlayerPrefs.SetInt("StepsSinceStart", stepsSinceStart);
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstLogin", 0);
        PlayerPrefs.Save();
        Debug.Log("First time setup: Initial steps recorded: " + stepsSinceStart);
    }

    private void Update()
    {
        currentSteps = stepCounterController.GetSteps();
        Debug.Log($"Current steps from controller: {currentSteps}, Previously recorded steps: {previousSteps}");

        if (currentSteps != previousSteps)
        {
            if (currentSteps > previousSteps)
            {
                int stepsToAdd = currentSteps - previousSteps;
                inGameSteps += stepsToAdd;
                Debug.Log($"Update: Steps added: {stepsToAdd}, New total in-game steps: {inGameSteps}");
                previousSteps = currentSteps; // Update previousSteps to the latest value
            }
            else
            {
                Debug.Log("Error: Current steps less than previous steps - check for reset or rollover");
            }
        }
        else
        {
            Debug.Log("No change in step count detected.");
        }
    }

    public int stepTokens;
    public void ConvertStepsToTokens()
    {
        stepTokens = (inGameSteps / 1000);
    }

    public bool firstTimeLogin = true;
    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("CurrentStageIndex", currentStageIndex);
        PlayerPrefs.SetInt("FirstLogin", firstTimeLogin ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    public void ResetSteps()
    {
        inGameSteps = 0;
        SaveStepsData();
        Debug.Log("In-game steps have been reset to 0.");
    }

    public void LoadPlayerData()
    {
        level = PlayerPrefs.GetInt("PlayerLevel", 1);
        gold = PlayerPrefs.GetInt("PlayerGold", 0);
        currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex", 0);
        firstTimeLogin = PlayerPrefs.GetInt("FirstLogin", 1) == 1;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerData();
            SaveStepsData();
        }
        else
        {
            LoadStepsData();
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
        SaveStepsData();
    }

    public void SaveStepsData()
    {
        PlayerPrefs.SetInt("InGameSteps", inGameSteps);
        PlayerPrefs.SetInt("PreviousSteps", previousSteps);
        PlayerPrefs.Save();
        Debug.Log("Saved steps data: InGameSteps and PreviousSteps");
    }

    public void LoadStepsData()
    {
        inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);
        previousSteps = PlayerPrefs.GetInt("PreviousSteps", 0);
        Debug.Log("Loaded steps data: InGameSteps and PreviousSteps");
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
}
