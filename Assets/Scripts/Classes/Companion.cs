using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class Companion : Character
{
    public abstract List<SkillType> AvailableSkills { get; }
    public abstract List<SkillType> LockedSkills { get; }
    public abstract List<SkillType> AllSkills { get; }
    public abstract Skill GetSkillInstance(SkillType skillType);
    public string heroID; // Name of the class/character 
    public int heroLevel; // Level of the hero
    public int heroExp;   // Experience of the hero
    public Sprite heroIcon;
    public int heroStatPoints;
    public int heroSkillPoints;
    public int stamina;
    public int maxStamina;
    public bool isUnlocked;
    public int atkGrowth;
    public int healthGrowth;
    public int energyGrowth;
    public Dictionary<string, int> statUpgradeProgress = new Dictionary<string, int>
    {
        {"atk", 0},
        {"hp", 0},
        {"spd", 0},
        {"sp",0}
        
    };



    
    //public Sprite companionIcon; // Icon associated with the companion

    
    
    [Serializable]
    public struct SerializableCharacterData
    {
      //  public int level;
        public int health;
        public int maxHealth;
        public int energy;
        public int maxEnergy;
        public int teamEnergy;
        public int attackPower;
        public int defensePower;
        public int defensePenetration;
        public int speed;
        public int expToLevel;
        public int heroSkillPoints;
        public int heroStatPoints;
        public int heroLevel;
        public int heroExp;
        public int stamina;
        public int maxStamina;
        
    }

    public int expToLevel
{
    get
    {
        
        return ExpToNextLevel(heroLevel);
    }
}



    //abstract LevelUp method
    public abstract void LevelUp();
    
    public void SaveCharacterData()
    {
        SerializableCharacterData data = new SerializableCharacterData
        {
            //level = this.level,
            health = this.health,
            maxHealth = this.maxHealth,
            maxEnergy = this.maxEnergy,
            attackPower = this.attackPower,
            defensePower = this.defensePower,
            defensePenetration = this.defensePenetration,
            speed = this.speed,
            heroLevel = this.heroLevel,
            heroExp = this.heroExp,
            heroSkillPoints = this.heroSkillPoints,
            heroStatPoints = this.heroStatPoints,
            stamina = this.stamina,
            maxStamina = this.maxStamina,
            
        };
        Debug.Log("saved" + health);
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("CharacterData_" + heroID, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadCharacterData()
{
    string jsonData = PlayerPrefs.GetString("CharacterData_" + heroID, "{}");
    if (jsonData != "{}") // Check if jsonData is not empty
    {
        SerializableCharacterData data = JsonUtility.FromJson<SerializableCharacterData>(jsonData);

       // this.level = data.level;
        this.health = data.health;
        this.maxHealth = data.maxHealth;
        this.maxEnergy = data.maxEnergy; 
        this.attackPower = data.attackPower; 
        this.defensePower = data.defensePower; 
        this.defensePenetration = data.defensePenetration; 
        this.speed = data.speed; 
        this.heroLevel = data.heroLevel;
        this.heroExp = data.heroExp;
        this.heroStatPoints = data.heroStatPoints;
        this.heroSkillPoints = data.heroSkillPoints;
        this.stamina = data.stamina;
        this.maxStamina = data.maxStamina;
    }
    else // Set default values if there is no data
    {
        this.stamina = 10;
        this.maxStamina = 10;
    }
}


    public void DeleteCharacterData()
{
    string key = "CharacterData_" + this.name;
    if (PlayerPrefs.HasKey(key))
    {
        PlayerPrefs.DeleteKey(key);
        Debug.Log("Deleted data for " + this.name);
    }
    else
    {
        Debug.Log("No data found for " + this.name);
    }
}


    public int ExpToNextLevel(int heroLevel)
{
    Debug.Log("EXP to level : " + (30 * heroLevel * heroLevel));
    //TEMP
     return 30 * heroLevel * heroLevel;
    /*
    if (heroLevel < 100)
    {
        // For levels 1-100, use a quadratic polynomial formula
        return 30 * heroLevel * heroLevel;
    }
    
    else
    {
        // Beyond level 20, use an exponential model to steeply increase EXP requirements
        return (int)(100 * Math.Pow(1.5, heroLevel - 19) * 400);
    }
    */
}

    public void RecoverHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health doesn't exceed maxHealth
    }

    public void RecoverEnergy(int amount)
    {
        if ((energy + amount) >= maxEnergy)
        {
            energy = maxEnergy;
        }
        else
        {
            energy += amount;
        }
    }

}

   


