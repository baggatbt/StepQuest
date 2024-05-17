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
    public int silver;
    public int copper;
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
            UpdateSteps(); //Compares the phones sensor from the last time it ran to now, adds steps if its greater
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
        UpdateSteps();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerData();
        }
        else
        {
            // Update the step offset based on the current steps count from the device and add difference to ingamesteps
            UpdateSteps();
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
    }


    private void UpdateSteps()
    {
            newSensorTotal = stepCounterController.GetSteps();
        int differenceBetweenSensorCounts = newSensorTotal - currentSensorTotal;
        //Debug.Log("Difference between sensor counts = " + differenceBetweenSensorCounts);

        if (differenceBetweenSensorCounts > 0)
        {
            //Add the steps to game, then reset the totalCount for next reboot
            inGameSteps += differenceBetweenSensorCounts;
            currentSensorTotal = newSensorTotal;
            //Debug.Log("updating old total, currentSensorTotal = " + currentSensorTotal);
     
        }
        else
        {
            currentSensorTotal = newSensorTotal;
           // Debug.Log("No new steps to load from sensor, updating old total, currentSensorTotal = " + currentSensorTotal);
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
    firstTimeLogin = PlayerPrefs.GetInt("FirstLogin", 0) == 1;

    // Load the saved step count
    inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);

    // Load the step count from the last reset to calculate the current step count
    currentSensorTotal = PlayerPrefs.GetInt("CurrentSensorTotal", currentSensorTotal);
    
    // The actual step count should be updated in the Update method using the current sensor value
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
    // Reset the in-game steps count
    inGameSteps = 0;

    // Immediately save this change to PlayerPrefs
    SavePlayerData();

    Debug.Log("In-game steps have been reset to 0.");
}


}
 