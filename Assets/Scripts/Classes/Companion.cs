using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Companion : Character
{
    public abstract List<SkillType> AvailableSkills { get; }
    public abstract List<SkillType> LockedSkills { get; }
    public abstract Skill GetSkillInstance(SkillType skillType);
    public string heroID; // Name of the class/character 
    public int heroLevel; // Level of the hero
    public int heroExp;   // Experience of the hero
    public int heroStatPoints;
    public int heroSkillPoints;
    public int stamina;
    public int maxStamina;
    public bool isUnlocked;
    
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
    if (heroLevel < 20)
    {
        // For levels 1-20, use a quadratic polynomial formula
        return 100 * heroLevel * heroLevel;
    }
    else
    {
        // Beyond level 20, use an exponential model to steeply increase EXP requirements
        return (int)(100 * Math.Pow(1.5, heroLevel - 19) * 400);
    }
}


/*EXP TO LEVEL, SOFT CAP AT 20
Level 1: 100 EXP
Level 2: 400 EXP
Level 3: 900 EXP
Level 4: 1600 EXP
Level 5: 2500 EXP
Level 6: 3600 EXP
Level 7: 4900 EXP
Level 8: 6400 EXP
Level 9: 8100 EXP
Level 10: 10000 EXP
Level 11: 12100 EXP
Level 12: 14400 EXP
Level 13: 16900 EXP
Level 14: 19600 EXP
Level 15: 22500 EXP
Level 16: 25600 EXP
Level 17: 28900 EXP
Level 18: 32400 EXP
Level 19: 36100 EXP
Level 20: 60000 EXP
Level 21: 90000 EXP
Level 22: 135000 EXP
Level 23: 202500 EXP
Level 24: 303750 EXP
Level 25: 455625 EXP
Level 26: 683437 EXP
Level 27: 1025156 EXP
Level 28: 1537734 EXP
Level 29: 2306601 EXP
Level 30: 3459902 EXP
*/
    

    
    
}

   


