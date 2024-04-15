using System;
using UnityEngine;
using System.Collections.Generic;

//HANDLES STORING/RETRIEVING OF ALL PLAYER RELATED DATA
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
    // public int dailySteps; // Future implementation for daily step tracking
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
    
   //For testing, wipes saved data 
  // PlayerPrefs.DeleteAll();

    Debug.Log("PlayerData Awake is running");
    if (_instance == null)
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure default value for first login
        if (!PlayerPrefs.HasKey("FirstLogin"))
        {
            firstTimeLogin = true;
        }
        
        LoadPlayerData();
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
        stepsSinceStart = stepCounterController.GetSteps(); // Get current step count from sensor
        PlayerPrefs.SetInt("StepsSinceStart", stepsSinceStart);
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstLogin", 0);
        PlayerPrefs.Save();
        Debug.Log("First time setup: Initial steps recorded: " + stepsSinceStart);
    }
    else
    {
        stepsSinceStart = PlayerPrefs.GetInt("StepsSinceStart", 0); // Load stepsSinceStart from PlayerPrefs
    }

    previousSteps = stepsSinceStart; // Initialize previousSteps to stepsSinceStart on every start
    int currentSteps = stepCounterController.GetSteps();
    inGameSteps = Math.Max(0, currentSteps - stepsSinceStart); // Ensure that inGameSteps doesn't go negative
    Debug.Log($"Game restarted: stepsSinceStart: {stepsSinceStart}, currentSteps: {currentSteps}, inGameSteps: {inGameSteps}");
}

private void Update()
{
    int currentSteps = stepCounterController.GetSteps();
    if (currentSteps >= previousSteps)
    {
        int stepsToAdd = currentSteps - previousSteps;
        inGameSteps += stepsToAdd; // Increment in-game steps by new steps detected since last update
        previousSteps = currentSteps;
        Debug.Log($"Update: Current steps: {currentSteps}, Previous steps: {previousSteps}, Steps to add: {stepsToAdd}, Total in-game steps: {inGameSteps}");
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
    PlayerPrefs.SetInt("FirstLogin", firstTimeLogin ? 1 : 0); // Convert bool to int
    
    PlayerPrefs.Save();
    Debug.Log("Data saved");
}

    public void ResetSteps()
{
    // Reset only the in-game steps counter
    inGameSteps = 0;

    // No need to touch stepsSinceStart since it's a historical record from first load

    // Save the reset state to PlayerPrefs to ensure persistence across sessions
    SaveStepsData();

    Debug.Log("In-game steps have been reset to 0.");
}


    public void LoadPlayerData()
{
    if (PlayerPrefs.HasKey("PlayerLevel"))
    {
        level = PlayerPrefs.GetInt("PlayerLevel");
        gold = PlayerPrefs.GetInt("PlayerGold");
        currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex");
        // Convert int back to bool
        firstTimeLogin = PlayerPrefs.GetInt("FirstLogin", 1) == 1; // Default to true if not set
    }
}


     private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            stepsAtCloseOfApp = inGameSteps;
            stepCounterController.SaveLastKnownSteps();
            SavePlayerData();
            SaveStepsData();
        }
        else
        {
            LoadStepsData();
            AdjustStepsPostPause();
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
        stepCounterController.SaveLastKnownSteps();
        SaveStepsData();
    }

    private void AdjustStepsPostPause()
    {
        int lastKnownSteps = stepCounterController.GetInitialStepsOnResume();
        int currentSteps = stepCounterController.GetSteps();
        if (currentSteps > lastKnownSteps)
        {
            inGameSteps += currentSteps - lastKnownSteps;
        }
        Debug.Log("Adjusted Steps after resume: " + inGameSteps);
    }

public void SaveStepsData()
{
    PlayerPrefs.SetInt("InGameSteps", inGameSteps);
    PlayerPrefs.Save();
    Debug.Log("Saved total steps: " + inGameSteps);
}

private void LoadStepsData()
{
    inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0); // Load in-game steps
    Debug.Log("Loaded Steps: " + inGameSteps);
}




  
}
