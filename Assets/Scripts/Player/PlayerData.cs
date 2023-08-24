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
    public int speed;
    public Dictionary<string, int> skillLevels;
    public Dictionary<string, int> skillExp;
    public List<Mission> activeMissions = new List<Mission>();

    private StepCounterController stepCounterController;

    private void Awake()
{
    if (_instance == null)
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Load the player data right here
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
        Debug.Log("Steps in Update: " + inGameSteps);
    }

    public void UpdatePlayerData(int level, int exp, int gold, int attackPower, int defensePower, int inGameSteps)
    {
        this.level = level;
        this.exp = exp;
        this.gold = gold;
        this.attackPower = attackPower;
        this.defensePower = defensePower;
        this.inGameSteps = inGameSteps;
        
        SavePlayerData(); 
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerExp", exp);
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.SetInt("PlayerAttackPower", attackPower);
        PlayerPrefs.SetInt("PlayerDefensePower", defensePower);
        PlayerPrefs.SetInt("PlayerInGameSteps", inGameSteps);
        PlayerPrefs.Save();
        Debug.Log("Data saved");
    }

    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            level = PlayerPrefs.GetInt("PlayerLevel");
            exp = PlayerPrefs.GetInt("PlayerExp");
            gold = PlayerPrefs.GetInt("PlayerGold");
            attackPower = PlayerPrefs.GetInt("PlayerAttackPower");
            defensePower = PlayerPrefs.GetInt("PlayerDefensePower");
            inGameSteps = PlayerPrefs.GetInt("PlayerInGameSteps");
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

    private void OnApplicationQuit()
    {
        SaveStepsData();
        SavePlayerData();
        Debug.Log("Saving steps on quit: " + inGameSteps);
    }

    private void LoadStepsData()
    {
        if(PlayerPrefs.HasKey("StepsBeforeClosing") && PlayerPrefs.HasKey("InGameSteps"))
        {
            int stepsBeforeClosing = PlayerPrefs.GetInt("StepsBeforeClosing", 0);
            int stepsWhenReOpening = stepCounterController.GetSteps();
            int previousInGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);
            int stepsDuringClosure = stepsWhenReOpening - stepsBeforeClosing;
            inGameSteps = previousInGameSteps + stepsDuringClosure; 
            previousSteps = stepsWhenReOpening;
            Debug.Log("Loaded Steps: " + inGameSteps);
        }
    }

    private void SaveStepsData()
    {
        PlayerPrefs.SetInt("StepsBeforeClosing", stepCounterController.GetSteps());
        PlayerPrefs.SetInt("InGameSteps", inGameSteps);
        PlayerPrefs.Save();
    }

    

    //NEEDS OWN CLASS
    // Method to increase the level of a skill
    public void IncreaseSkillLevel(string skillName, int amount)
    {
        if (skillLevels.ContainsKey(skillName))
        {
            skillLevels[skillName] += amount; // Usage: PlayerData.Instance.IncreaseSkillLevel("Slash", 1);  // Increase the level of Slash skill by 1

        }
        else
        {
            skillLevels[skillName] = amount;
        }
    }

     // Method to increase the experience of a skill
    public void IncreaseSkillExp(string skillName, int amount)
    {
        if (skillExp.ContainsKey(skillName))
        {
            skillExp[skillName] += amount;
        }
        else
        {
            skillExp[skillName] = amount;
        }
    }
    


    //MOVE THESE TO ANOTHER CLASS
     public void ActivateMission(Mission mission)
    {
        mission.ActivateMission();
        if (!activeMissions.Contains(mission))
        {
            activeMissions.Add(mission);
        }
    }

    public void RemoveMission(Mission mission)
    {
        if (activeMissions.Contains(mission))
        {
            activeMissions.Remove(mission);
        }
    }
}
