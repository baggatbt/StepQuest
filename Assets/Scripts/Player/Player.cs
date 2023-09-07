using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public PlayerJob CurrentJob { get; private set; }


    private PlayerData playerData;
    private int expRequiredToLevel;

    protected override void Awake()
    {
        base.Awake();
        playerData = PlayerData.Instance;
        if (PlayerPrefs.HasKey("PlayerInitialized"))
        {
            LoadFromPlayerData();
            Debug.Log("Already init'd, this is loading");
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

            PlayerPrefs.SetInt("PlayerInitialized", 1);
            PlayerPrefs.Save();

            // Save the initialized data
            playerData.SavePlayerData();
            SetJob(playerData.jobClass);
            ApplyJobStats();
            SyncStatsWithPlayerData();
            UpdateUI();
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
    
    }


     private void SetJob(string jobClassName)
    {
        switch (jobClassName)
        {
            case "Knight":
                CurrentJob = gameObject.AddComponent<Knight>();
                break;
            // Add other cases for other jobs...
            default:
                Debug.LogError("Unknown job class: " + jobClassName);
                break;
        }
    }
    public void ChangeJob(string newJob)
    {
        if (CurrentJob != null) Destroy(CurrentJob);

        SetJob(newJob);
        ApplyJobStats();
    }

    private void ApplyJobStats()
    {
        playerData.attackPower = CurrentJob.BaseAtk;
        playerData.defensePower = CurrentJob.BaseDef;
        playerData.maxHealth = CurrentJob.BaseHealth;
        playerData.maxEnergy = CurrentJob.BaseEnergy;
        // Handle magic attack and defense here...

        playerData.SavePlayerData();
        SyncStatsWithPlayerData();
    }

    private void LoadFromPlayerData()
{
    playerData.LoadPlayerData();
    SyncStatsWithPlayerData();
    UpdateUI();
}


    private void Update()
    {
        
        // Update UI regularly or as per your needs
       // UpdateUI();
        
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
}


    
    public void RewardGold(int amount)
    {
        playerData.gold += amount;
        playerData.SavePlayerData();
    }

    public void GainExperience(int amount)
    {
        playerData.exp += amount;
        playerData.SavePlayerData();
        LevelUp();
        

    }

    public void LevelUp()
    {
    if (playerData.exp >= ExpRequiredToLevelUp(playerData.level))
        {
        Debug.Log("Level up");
        playerData.level += 1;

        ApplyJobStats();
        }
    }

    //ExpRequiredToLevelUp(1) will return 25.
    //ExpRequiredToLevelUp(2) will return 100.
    //ExpRequiredToLevelUp(50) will return 62,500.
    private int ExpRequiredToLevelUp(int level)
{
    int a = 25; // This constant can be adjusted based on your needs.
    
    int expRequiredToLevel = a * level * level;
    
    return expRequiredToLevel;
}


    


}









