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

    public int level;
    public int exp;
    public int gold;
    public int attackPower;
    public int defensePower;
    public int inGameSteps;
    public int stepsSinceStart; //Steps since the game was started once
   // public int dailySteps; //Tracker for the 10k steps a day that will reset NOT IMPLEMENTED
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
   //PlayerPrefs.DeleteAll();

    if (_instance == null)
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        
        LoadPlayerData();
        

        
        
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
       
        LoadStepsData();
       
        int stepsSinceStart = stepCounterController.GetStepsSinceStart();
        inGameSteps += stepsSinceStart;
        Debug.Log("Steps after adding stepsSinceStart: " + inGameSteps);
    }

    private int previousSteps = 0;

    private void Update()
    {
        int currentSteps = stepCounterController.GetSteps();
        if(currentSteps >= previousSteps)
        {
            inGameSteps += currentSteps - previousSteps;
            previousSteps = currentSteps;
        }

        
     //   Debug.Log("Steps in Update: " + inGameSteps);
    }

   

    

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("CurrentStageIndex", currentStageIndex);
       
       
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            level = PlayerPrefs.GetInt("PlayerLevel");
            gold = PlayerPrefs.GetInt("PlayerGold");
            currentStageIndex = PlayerPrefs.GetInt("CurrentStageIndex");
            
            
        }
    }

    private void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
    {
        SaveStepsData();
    }
    else
    {
        LoadStepsData();
    }
}

private void SaveStepsData()
{
    // Save the total steps from the plugin when the app is paused or closed
    PlayerPrefs.SetInt("TotalStepsWhenClosed", stepCounterController.GetSteps());
    PlayerPrefs.Save();
    Debug.Log("Saved total steps when closed: " + stepCounterController.GetSteps());
}

private void LoadStepsData()
{
    int totalStepsWhenClosed = PlayerPrefs.GetInt("TotalStepsWhenClosed", stepCounterController.GetSteps());
    int totalStepsNow = stepCounterController.GetSteps();

    // Calculate the steps taken after the game was started
    inGameSteps = totalStepsNow - totalStepsWhenClosed;

    Debug.Log("Loaded Steps: " + inGameSteps);
}



private void OnApplicationQuit()
{
    SaveStepsData();
}





  
}
