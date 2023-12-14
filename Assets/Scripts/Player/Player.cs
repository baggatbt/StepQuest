/*
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



    private PlayerData playerData;
    private int expRequiredToLevel;
    private int stepsAvailableToConsume;
    private int exp;
    private int gold;
    private string heroID;
    private int inGameSteps;
    


    protected override void Awake()
    {
        base.Awake();
        playerData = PlayerData.Instance;
        if (PlayerPrefs.HasKey("PlayerInitialized"))
        {
           
           
        }
        else
        {
            InitializePlayer();
           
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
            playerData.heroID = "Knight";
            playerData.exp = 0;
            playerData.gold = 100;
            playerData.attackPower = 2;
            playerData.defensePower = 1;
            playerData.inGameSteps = 0;
            playerData.maxHealth = 10;
            playerData.health = playerData.maxHealth;
            playerData.maxEnergy = 10;
            playerData.energy = maxEnergy;
            playerData.teamEnergy = 0; //Set to zero to mimic the new system of just a shared AP bar.
            playerData.speed = 10;
            

            PlayerPrefs.SetInt("PlayerInitialized", 1);
            

             // Save the initialized data
            
            UpdateUI();
            PlayerPrefs.Save();
        }
        else{
            
        }
    }

   
    
    


    public void DeleteEverything()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Player prefs deleted");
    }




    private void Update()
    {
       
       UpdateUI();
      // playerData.UpdatePlayerData();
       
  
    }

    private void UpdateUI()
{
    
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
                healthText.text = health + " / " + maxHealth;
                
            }}
         
}


    
    public void GainGold(int amount)
    {
        playerData.gold += amount;
       
    }

    public void GainExperience(int amount)
    {
        playerData.exp += amount;
        
       
    }


}



*/





