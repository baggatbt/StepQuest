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
    public Button saveButton;
    public Button loadButton;
    public int exp;
    public int gold;
    public int level;

    protected override void Awake()
{
    base.Awake();
        this.level = 1;
        this.attackPower = 2;
        this.maxHealth = 10;
        this.health = maxHealth;
        this.maxEnergy = 10;
        this.energy = maxEnergy;
        this.defensePower = 0;
    
    }

    void Update()
{
    // Get the data from the PlayerData instance.
    PlayerData data = PlayerData.Instance;

    // Convert the level, exp, and gold int values to strings and update the text fields.
    if(levelText != null)
    {
        levelText.text = "Level: " + data.level.ToString();
    }

    if(expText != null)
    {
        expText.text = "EXP: " + data.exp.ToString();
    }

    if(goldText != null)
    {
        goldText.text = "Gold: " + data.gold.ToString();
    }

    if(attackPowerText != null)
    {
        attackPowerText.text = "AP: " + data.attackPower.ToString();
    }

    if(defensePowerText != null)
    {
        defensePowerText.text = "DP: " + data.defensePower.ToString();
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


}


   

