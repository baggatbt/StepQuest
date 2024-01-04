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
    
    
    
    [Serializable]
    public struct SerializableCharacterData
    {
        public int level;
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
    }


    //abstract LevelUp method
    public abstract void LevelUp();
    
    public void SaveCharacterData()
    {
        SerializableCharacterData data = new SerializableCharacterData
        {
            level = this.level,
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

        this.level = data.level;
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
    }
    else // Set default values if there is no data
    {
        
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
    // Parameters for levels 1-90
    int baseExp = 100; // Base experience for the first level
    int incrementPerLevel = 100; // Additional experience required for each subsequent level
    int expRequiredToLevel = baseExp + (incrementPerLevel * heroLevel);
    return expRequiredToLevel;
}

    
    
}

   


