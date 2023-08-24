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

    private PlayerData playerData;
    private int expRequiredToLevel;

    protected override void Awake()
    {
        base.Awake();
        playerData = PlayerData.Instance;
    }

    new private void  Start()
    {
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

    private void InitializePlayer()
    {
        if (!PlayerPrefs.HasKey("PlayerInitialized"))
        {
            playerData.level = 1;
            playerData.jobClass = "Adventurer";
            playerData.exp = 0;
            playerData.gold = 100;
            playerData.attackPower = 10;
            playerData.defensePower = 5;
            playerData.inGameSteps = 0;

            PlayerPrefs.SetInt("PlayerInitialized", 1);
            PlayerPrefs.Save();

            // Save the initialized data
            playerData.SavePlayerData();
        }
    }

    private void LoadFromPlayerData()
    {
        playerData.LoadPlayerData();
        Debug.Log(playerData.jobClass);
    }

    private void Update()
    {
        
        // Update UI regularly or as per your needs
        UpdateUI();
        
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
          StatGrowthForLevelUp();
          playerData.SavePlayerData();
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


    public void StatGrowthForLevelUp()
{
    switch(playerData.jobClass)
    {
        case "Adventurer":
            playerData.attackPower += Adventurer.atkGrowth;
            playerData.defensePower += Adventurer.defGrowth;
            //playerData.magAttackPower += Adventurer.magAtkGrowth;
            //playerData.magDefPower += Adventurer.magDefGrowth;
            break;

        case "Knight":
            Debug.Log("Not implemented");
            break;

        default:
            Debug.LogWarning("Unknown job class: " + playerData.jobClass);
            break;
    }
}


}









