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
    public int stepsSinceStart;
    public int maxHealth;
    public int health;
    public int maxEnergy;
    public int energy;
    public int maxTempEnergy;
    public int tempEnergy;
    public string jobClass;
    public int speed;
    
    //Knight specific
    public int knightSkillPoints;
    public int knightLevel;
    public int knightExp;
    
    
    public Dictionary<string, int> skillLevels;
    public Dictionary<string, int> skillExp;
    public List<Mission> activeMissions = new List<Mission>();

    private StepCounterController stepCounterController;
    

    private void Awake()
{
    maxTempEnergy = (maxEnergy / 2);
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

        maxTempEnergy = (maxEnergy / 2);
        tempEnergy = 0;
        stepCounterController = FindObjectOfType<StepCounterController>();
        if (stepCounterController == null)
        {
            Debug.LogError("StepCounterController not found in the scene!");
        }
    }

    private Dictionary<string, PlayerJob> jobInstances = new Dictionary<string, PlayerJob>
    {
        {"Knight", new Knight()} // Add other job instances as necessary
    };

    public PlayerJob CurrentJob
    {
        get 
        {
            if (jobInstances.ContainsKey(jobClass))
                return jobInstances[jobClass];

            Debug.LogError("Job instance for " + jobClass + " not found.");
            return null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;  // Set target frame rate to 60 FPS.
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

    public void UpdatePlayerData(int level, string jobClass, int exp, int gold, int attackPower, int defensePower, int inGameSteps, int maxHealth, int health, int maxEnergy, int energy, int speed, int maxTempEnergy, int tempEnergy)
    {
        this.level = level;
        this.jobClass = jobClass;
        this.exp = exp;
        this.gold = gold;
        this.attackPower = attackPower;
        this.defensePower = defensePower;
        this.inGameSteps = inGameSteps;
        this.maxHealth = maxHealth;
        this.health = health;
        this.maxEnergy = maxEnergy;
        this.energy = energy;
        this.speed = speed;
        this.maxTempEnergy = maxTempEnergy;
        this.tempEnergy = tempEnergy;
        

        SavePlayerData(); 
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetString("PlayerJob", jobClass);
        PlayerPrefs.SetInt("PlayerExp", exp);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("PlayerAttackPower", attackPower);
        PlayerPrefs.SetInt("PlayerDefensePower", defensePower);
        PlayerPrefs.SetInt("PlayerInGameSteps", inGameSteps);
        PlayerPrefs.SetInt("PlayerMaxHealth", maxHealth);
        PlayerPrefs.SetInt("PlayerMaxEnergy", maxEnergy); 
        PlayerPrefs.SetInt("PlayerSpeed", speed);
        PlayerPrefs.SetInt("PlayerMaxtempEnergy", maxTempEnergy);
        PlayerPrefs.SetInt("KnightSkillPoints",knightSkillPoints);
        PlayerPrefs.SetInt("KnightLevel",knightLevel);
        PlayerPrefs.SetInt("KnightExp",knightExp);
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            level = PlayerPrefs.GetInt("PlayerLevel");
            jobClass = PlayerPrefs.GetString("PlayerJob");
            exp = PlayerPrefs.GetInt("PlayerExp");
            gold = PlayerPrefs.GetInt("PlayerGold");
            attackPower = PlayerPrefs.GetInt("PlayerAttackPower");
            defensePower = PlayerPrefs.GetInt("PlayerDefensePower");
            inGameSteps = PlayerPrefs.GetInt("PlayerInGameSteps");
            maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth");
            maxEnergy = PlayerPrefs.GetInt("PlayerMaxEnergy");
            speed = PlayerPrefs.GetInt("PlayerSpeed");
            knightSkillPoints = PlayerPrefs.GetInt("KnightSkillPoints");
            knightLevel = PlayerPrefs.GetInt("KnightLevel");
            knightExp = PlayerPrefs.GetInt("KnightExp");
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
    int totalStepsWhenClosed = PlayerPrefs.GetInt("TotalStepsWhenClosed", 0);
    int totalStepsNow = stepCounterController.GetSteps();
    int stepsTakenWhileAppClosed = totalStepsNow - totalStepsWhenClosed;

    // Add the steps taken while the app was closed to the in-game steps
    inGameSteps += stepsTakenWhileAppClosed;
    Debug.Log("Loaded Steps: " + inGameSteps);
}


private void OnApplicationQuit()
{
    SaveStepsData();
}





  
}
