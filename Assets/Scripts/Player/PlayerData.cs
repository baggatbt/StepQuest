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
    public int inGameSteps; //Steps used for the game
    public int currentSensorTotal; //What the phone says the total is
    public int newSensorTotal; // On reboot of game, what the new total on sensor is so I can convert to inGame

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

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeStepCounter();
            LoadPlayerData(); // Load player data first

            if (firstTimeLogin)
            {
                HandleFirstLogin();
            }
            else
            {
                UpdateSteps(); // Only update steps if it's not the first time login
            }

            Debug.Log("PlayerData Awake complete");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure any first login tasks are handled in Awake
    }

    private void Update()
    {
        if (!firstTimeLogin) // Ensure steps are not updated on the first login
        {
            UpdateSteps();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerData();
        }
        else
        {
            if (!firstTimeLogin)
            {
                UpdateSteps(); // Update steps only if it's not the first time login
            }
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
    }

    private void HandleFirstLogin()
    {
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstTimeLogin", firstTimeLogin ? 1 : 0);
        inGameSteps = 0;
        currentSensorTotal = stepCounterController.GetSteps();
        SavePlayerData();

        Debug.Log("First time login handled, step data initialized.");
    }

    private void UpdateSteps()
    {
        newSensorTotal = stepCounterController.GetSteps();
        int differenceBetweenSensorCounts = newSensorTotal - currentSensorTotal;

        if (differenceBetweenSensorCounts > 0)
        {
            inGameSteps += differenceBetweenSensorCounts;
            currentSensorTotal = newSensorTotal;
        }
        else
        {
            currentSensorTotal = newSensorTotal;
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("CurrentStageIndex", currentStageIndex);

        // Save the current step count when saving player data
        PlayerPrefs.SetInt("InGameSteps", inGameSteps);
        PlayerPrefs.SetInt("CurrentSensorTotal", currentSensorTotal);

        Debug.Log("Saving Player Data with in-game steps: " + inGameSteps);
        Debug.Log("Saving Player Data with sensor total: " + currentSensorTotal);

        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        level = PlayerPrefs.GetInt("PlayerLevel", 1);
        gold = PlayerPrefs.GetInt("PlayerGold", 0);
        currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex", 0);
        firstTimeLogin = PlayerPrefs.GetInt("FirstTimeLogin", 1) == 1;

        // Load the saved step count
        inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);

        // Load the step count from the last reset to calculate the current step count
        currentSensorTotal = PlayerPrefs.GetInt("CurrentSensorTotal", 0);

        Debug.Log("Loading Player Data with in-game steps: " + inGameSteps);
    }

    public bool UseSteps(int amountToUse)
    {
        if (inGameSteps >= amountToUse)
        {
            inGameSteps -= amountToUse;
            Debug.Log("Spent steps: " + amountToUse);
            SavePlayerData();
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