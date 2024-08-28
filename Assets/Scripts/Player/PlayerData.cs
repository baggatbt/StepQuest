using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

   

public class PlayerData : MonoBehaviour
{
    private static PlayerData _instance;
    public static PlayerData Instance => _instance;

    public int level;
    public int exp;
    public int gold;
    public int attackPower;
    public int defensePower;
    public int inGameSteps; // Steps used for the game
    public int baselineSteps; // The step count recorded at first login
    public int currentSensorTotal; // The current total steps reported by the sensor

    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int teamEnergy;
    public string heroID;
    public int speed;
    public int currentStageIndex;
    public bool firstTimeLogin = true;

    private StepCounterController stepCounterController;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeStepCounter();
            LoadPlayerData();

            if (firstTimeLogin)
            {
                HandleFirstLogin();
            }
            else
            {
                currentSensorTotal = stepCounterController.GetTotalSteps(); // Set current sensor total from the device
                UpdateSteps(); // Update steps only if it's not the first time login
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateSteps();
    }

    private void InitializeStepCounter()
    {
        stepCounterController = StepCounterController.Instance;
        if (stepCounterController == null)
        {
            Debug.LogError("StepCounterController not found in the scene!");
        }
    }

    private void HandleFirstLogin()
    {
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstTimeLogin", firstTimeLogin ? 1 : 0);

        // Capture the baseline steps on first login
        baselineSteps = stepCounterController.GetTotalSteps();
        PlayerPrefs.SetInt("BaselineSteps", baselineSteps);

        inGameSteps = 0;
        currentSensorTotal = baselineSteps; // Initialize the current sensor total with the baseline
        SavePlayerData();

        Debug.Log("First time login handled, step data initialized.");
    }
    public int newSteps;
    public int newSensorTotal;
    private void UpdateSteps()
    {
        newSensorTotal = stepCounterController.GetTotalSteps();
        newSteps = newSensorTotal - currentSensorTotal; // Calculate new steps since last update

        Debug.Log($"Baseline Steps: {baselineSteps}");
        Debug.Log($"Current Sensor Total: {currentSensorTotal}");
        Debug.Log($"New Sensor Total: {newSensorTotal}");
        Debug.Log($"New Steps Calculated: {newSteps}");

        if (newSteps > 0)
        {
            inGameSteps += newSteps; // Add new steps to the in-game total
            currentSensorTotal = newSensorTotal; // Update the current sensor total
            SavePlayerData();
        }
        else
        {
            Debug.Log("No new steps detected.");
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("CurrentStageIndex", currentStageIndex);

        // Save the current step count when saving player data
        PlayerPrefs.SetInt("InGameSteps", inGameSteps);
        PlayerPrefs.SetInt("BaselineSteps", baselineSteps);
        PlayerPrefs.SetInt("CurrentSensorTotal", currentSensorTotal);

        Debug.Log("Player data saved.");
        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        level = PlayerPrefs.GetInt("PlayerLevel", 1);
        gold = PlayerPrefs.GetInt("PlayerGold", 0);
        currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex", 0);
        firstTimeLogin = PlayerPrefs.GetInt("FirstTimeLogin", 1) == 1;

        inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);
        baselineSteps = PlayerPrefs.GetInt("BaselineSteps", 0);
        currentSensorTotal = PlayerPrefs.GetInt("CurrentSensorTotal", 0);

        Debug.Log("Player data loaded.");
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
