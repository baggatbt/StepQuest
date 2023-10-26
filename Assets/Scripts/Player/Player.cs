using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Player : Character
{
    public Transform playerSpawnPoint;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI defensePowerText;
    public TextMeshProUGUI magicAttackText;
    public TextMeshProUGUI magicDefenseText;
    public TextMeshProUGUI stepsText;
    public TextMeshProUGUI jobText;
    public TextMeshProUGUI knightSkillPointsText;
    public PlayerJob CurrentJob { get; private set; }


    private PlayerData playerData;
    private int expRequiredToLevel;
    private int stepsAvailableToConsume;
    private int exp;
    private int gold;
    private string jobClass;
    private int inGameSteps;
    


    protected override void Awake()
    {
        base.Awake();
        playerData = PlayerData.Instance;
        if (PlayerPrefs.HasKey("PlayerInitialized"))
        {
            LoadFromPlayerData();
           
        }
        else
        {
            InitializePlayer();
            Debug.Log("making new player");
        }

        UpdateUI();  // Update the UI with initial values.
    }

    new private void  Start()
    {
       SyncStatsWithPlayerData();
    }

    private void InitializePlayer()
    {
        if (!PlayerPrefs.HasKey("PlayerInitialized"))
        {
            playerData.level = 1;
            playerData.jobClass = "Knight";
            playerData.exp = 0;
            playerData.gold = 100;
            playerData.attackPower = 3;
            playerData.defensePower = 5;
            playerData.inGameSteps = 0;
            playerData.maxHealth = 10;
            playerData.health = playerData.maxHealth;
            playerData.maxEnergy = 5;
            playerData.energy = playerData.maxEnergy;
            playerData.speed = 3;
            playerData.maxTempEnergy = playerData.maxEnergy;
            playerData.tempEnergy = 0;

            PlayerPrefs.SetInt("PlayerInitialized", 1);
            

             // Save the initialized data
            playerData.SavePlayerData();
            SetJob(playerData.jobClass);
            ApplyJobStats();
            SyncStatsWithPlayerData();
            UpdateUI();
            PlayerPrefs.Save();
        }
        else{
            LoadFromPlayerData();
        }
    }

     private void SyncStatsWithPlayerData()
    {
    maxHealth = playerData.maxHealth;
    health = maxHealth;
    maxEnergy = playerData.maxEnergy; 
    energy = maxEnergy;
    attackPower = playerData.attackPower;
    defensePower = playerData.defensePower;
    exp = playerData.exp;
    jobClass = playerData.jobClass;
    inGameSteps = playerData.inGameSteps;
    tempEnergyMax = playerData.maxTempEnergy;
    tempEnergy = playerData.tempEnergy;
    
    
    }


    public void DeleteEverything()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player prefs deleted");
    }


     private void SetJob(string jobClassName)
    {
        switch (jobClassName)
        {
            case "Knight":
                CurrentJob = new Knight();
                break;
            // Add other cases for other jobs...
            default:
                Debug.LogError("Unknown job class: " + jobClassName);
                break;
        }
    }
    public void ChangeJob(string newJob)
    {
        SetJob(newJob);
        ApplyJobStats();
    }

    private void ApplyJobStats()
    {
       
        CurrentJob.JobLevel += 1;

        playerData.SavePlayerData();
        SyncStatsWithPlayerData();
    }

    private void LoadFromPlayerData()
{
    playerData.LoadPlayerData();
    SetJob(playerData.jobClass);  // Set the job based on the loaded data
    SyncStatsWithPlayerData();
    UpdateUI();
}



    private void Update()
    {
       
       UpdateUI();
      // playerData.UpdatePlayerData();
      
       
        
    }

    private void UpdateUI()
{
    if (jobText != null) jobText.text = "Job: " + playerData.jobClass;
    if (levelText != null) levelText.text = "Level: " + playerData.level.ToString();
    if (expText != null) expText.text = "Exp: " + playerData.exp.ToString();
    if (goldText != null) goldText.text = "Gold: " + playerData.gold.ToString();
    if (attackPowerText != null) attackPowerText.text = "Atk:" + playerData.attackPower.ToString();
    if (defensePowerText != null) defensePowerText.text = "Def: " + playerData.defensePower.ToString();
    if (stepsText != null) stepsText.text = "Steps: " + playerData.inGameSteps.ToString();
    if (magicAttackText != null) magicAttackText.text = "M.Atk: 0";
    if (magicDefenseText != null) magicDefenseText.text = "M.Def: 0";
    if (knightSkillPointsText != null) knightSkillPointsText.text = "SP: " + playerData.knightSkillPoints.ToString();

    if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            if (healthText != null )
            {
                healthText.text = "HP: " + health; 
                
            }}
            LevelUp();
}


    
    public void GainGold(int amount)
    {
        playerData.gold += amount;
        playerData.SavePlayerData();
    }

    public void GainExperience(int amount)
    {
        playerData.exp += amount;
        LevelUp();
        playerData.SavePlayerData();
    }

   public void ConsumeStepsToGainRewards()
{
    stepsAvailableToConsume = PlayerData.Instance.stepsSinceStart;
    Debug.Log(stepsAvailableToConsume); 

    int expReward = (int)Math.Round(stepsAvailableToConsume * 0.01);
    int goldReward = (int)Math.Round(stepsAvailableToConsume * 0.01);

    GainExperience(expReward);
    GainGold(goldReward);
    PlayerData.Instance.stepsSinceStart = 0;
    playerData.SavePlayerData();
}


    public void LevelUp()
    {
        if (CurrentJob.JobExp >= ExpRequiredToLevelUp(CurrentJob.JobLevel))
            {
                Debug.Log("Level up");
                CurrentJob.JobLevel += 1;
                ApplyJobStats();
                playerData.SavePlayerData();
                UpdateUI();
            }
    }

    //ExpRequiredToLevelUp(1) will return 25.
    //ExpRequiredToLevelUp(2) will return 100.
    //(3) will return 225
    //ExpRequiredToLevelUp(50) will return 62,500.
    private int ExpRequiredToLevelUp(int level)
{
    int a = 25; 
    
    int expRequiredToLevel = a * level * level;
    
    return expRequiredToLevel;
}


    


}









