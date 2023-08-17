using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Namespace for TextMeshPro


[System.Serializable]
public class Player : Character
{
    public Transform playerSpawnPoint;
    // The TextMeshProUGUI references.
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI defensePowerText;
    public TextMeshProUGUI stepsText;
    public Button saveButton;
    public Button loadButton;
    public int level;
    public int exp;
    public int gold;
    public int steps;

   protected override void Awake()
    {
        base.Awake();
        InitializePlayer();

        // Initialize PlayerData with the current Player values
        PlayerData.Instance.Initialize(this);
        
    }

    private void InitializePlayer()
    {
        level = 1;
        exp = 0;
        gold = 0;
        attackPower = 2;
        maxHealth = 10;
        health = maxHealth;
        maxEnergy = 5;
        energy = maxEnergy;
        defensePower = 0;
        speed = 2;
        steps = 0;
    }

    
     void Update()
    {
        UpdateUI();
    }

    public void UpdateSteps(int newSteps)
    {
        this.steps = newSteps;
        PlayerData.Instance.steps = this.steps;
    }

    private void UpdateUI()
    {
        PlayerData data = PlayerData.Instance;

        if (levelText) levelText.text = "Level: " + data.level;
        if (expText) expText.text = "EXP: " + data.exp;
        if (goldText) goldText.text = "Gold: " + data.gold;
        if (attackPowerText) attackPowerText.text = "Atk: " + data.attackPower;
        if (defensePowerText) defensePowerText.text = "Def: " + data.defensePower;
        if (stepsText) stepsText.text = "Steps: " + data.steps;
    }
}









    /* Implement the saving and loading functions
    public void SavePlayerData()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            level = data.level;
            exp = data.exp;
            gold = data.gold;
            attackPower = data.attackPower;
            defensePower = data.defensePower;
        }
        Update();
    }
    */





   

