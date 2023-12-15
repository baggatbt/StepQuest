using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Companion : Character
{
    public abstract List<SkillType> AvailableSkills { get; }
    public abstract List<SkillType> LockedSkills { get; }
    public abstract Skill GetSkillInstance(SkillType skillType);
    public string heroID; //Name of the class/character 
    public int heroSkillPoints;
    public int heroStatPoints;
    public int heroLevel;
    public int heroExp;
    
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
        // Include any other relevant fields.
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
    this.maxEnergy = data.maxEnergy; // Changed comma to semicolon
    this.attackPower = data.attackPower; // Changed comma to semicolon
    this.defensePower = data.defensePower; // Changed comma to semicolon
    this.defensePenetration = data.defensePenetration; // Changed comma to semicolon
    this.speed = data.speed; // Changed comma to semicolon
}

    }

   


