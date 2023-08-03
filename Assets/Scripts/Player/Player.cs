using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Namespace for TextMeshPro


[System.Serializable]
public class Player : Character
{
    public Transform playerSpawnPoint;
    public PlayerStats playerStats;
    // The TextMeshProUGUI references.
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;
    public int exp;
    public int gold;
    public int level;

    protected override void Awake()
{
    base.Awake();
        
        this.attackPower = 2;
        this.maxHealth = 10;
        this.health = maxHealth;
        this.maxEnergy = 10;
        this.energy = maxEnergy;
        this.defensePower = 0;
    
    }

    void Update()
{
    // Convert the level, exp, and gold int values to strings and update the text fields.
    if(levelText != null)
    {
        levelText.text = "Level: " + level.ToString();
    }

    if(expText != null)
    {
        expText.text = "EXP: " + exp.ToString();
    }

    if(goldText != null)
    {
        goldText.text = "Gold: " + gold.ToString();
    }
}
}


   

