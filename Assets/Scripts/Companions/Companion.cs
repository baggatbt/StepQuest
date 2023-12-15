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
            
        };

        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("CharacterData_" + this.name, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadCharacterData()
{
    string jsonData = PlayerPrefs.GetString("CharacterData_" + this.name, "{}");
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
}

    public int ExpToNextLevel(int heroLevel)
{
    // Parameters for levels 1-90
    int baseExp = 100; // Base experience for the first level
    int incrementPerLevel = 100; // Additional experience required for each subsequent level

    // Parameter for levels 91-100
    int expPerLevel91To100 = 81900; // Flat experience required for each level from 91 to 100

    // Calculate experience for levels 1-90 using linear growth
    if (heroLevel >= 1 && heroLevel <= 90)
    {
        return baseExp + (heroLevel - 1) * incrementPerLevel;
    }
    // Experience for levels 91-100
    else if (heroLevel >= 91 && heroLevel <= 100)
    {
        return expPerLevel91To100;
    }
    else
    {
        // Handle levels outside 1-100, if necessary
        return 0;
    }
}




    }

   


