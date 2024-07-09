using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public abstract class Companion : Character
{
    [SerializeField]
    protected List<SkillType> availableSkills = new List<SkillType>();
    protected List<SkillType> lockedSkills = new List<SkillType>();

    public virtual List<SkillType> AvailableSkills => availableSkills;
    public virtual List<SkillType> LockedSkills => lockedSkills;
    public abstract List<SkillType> AllSkills { get; }
    public abstract List<SkillType> MainSkills { get; }
    public abstract Skill GetSkillInstance(SkillType skillType);

    public string heroID;
    public int heroLevel;
    public int heroExp;
    public GameObject skillTreePanel;
    public Sprite heroIcon;
    public Sprite fullHeroImage;
    public int heroStatPoints;
    public int heroSkillPoints;
    public int stamina;
    public int maxStamina;
    public bool isUnlocked;
    public int atkGrowth;
    public int healthGrowth;
    public int energyGrowth;
    //Implementing Scriptable Objects for hero data
    public CharacterData characterData;
    public Dictionary<string, int> statUpgradeProgress = new Dictionary<string, int>
    {
        {"atk", 0},
        {"hp", 0},
        {"spd", 0},
        {"sp", 0}
    };
    public Dictionary<EquipmentType, Equipment> equippedItems = new Dictionary<EquipmentType, Equipment>();

    [Serializable]
    public struct SerializableCharacterData
    {
        public int health;
        public int maxHealth;
        public int energy;
        public int maxEnergy;
        public int attackPower;
        public int defensePower;
        public int defensePenetration;
        public int speed;
        public int heroLevel;
        public int heroExp;
        public int heroSkillPoints;
        public int heroStatPoints;
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

    protected override void Awake()
    {
        base.Awake();
        
    }

    public void SetCharacterData(CharacterData data)
    {
        characterData = data;
        InitializeCharacterStats();
    }

   public void InitializeCharacterStats()
    {
        if (characterData != null)
        {
            heroID = characterData.heroID;
            heroLevel = characterData.heroLevel;
            heroExp = characterData.heroExp;
            skillTreePanel = characterData.skillTreePanel;
            heroIcon = characterData.heroIcon;
            fullHeroImage = characterData.fullHeroImage;
            heroStatPoints = characterData.heroStatPoints;
            heroSkillPoints = characterData.heroSkillPoints;
            stamina = characterData.stamina;
            maxStamina = characterData.maxStamina;
            maxHealth = characterData.maxHealth;
            health = characterData.health;
            maxEnergy = characterData.maxEnergy;
            energy = characterData.energy;
            speed = characterData.speed;
            // Load other stats and methods from characterData
        }
        else
        {
            Debug.LogError("Character data is not assigned.");
        }
    }

    

    public abstract void LevelUp();

    public abstract void InitializeSkillsBasedOnLevel();

    public void UnlockSkill(SkillType skillType)
    {
        if (LockedSkills.Contains(skillType))
        {
            LockedSkills.Remove(skillType);
            AvailableSkills.Add(skillType);
            Debug.Log(skillType.ToString() + " unlocked.");
        }
        else
        {
            Debug.LogError(skillType.ToString() + " is not in the LockedSkills list.");
        }
    }

    public void SaveCharacterData()
    {
        SerializableCharacterData data = new SerializableCharacterData
        {
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
        if (jsonData != "{}")
        {
            SerializableCharacterData data = JsonUtility.FromJson<SerializableCharacterData>(jsonData);
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
        else
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
        return 30 * heroLevel * heroLevel;
    }

    public void RecoverHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void RecoverEnergy(int amount)
    {
        energy = Mathf.Min(energy + amount, maxEnergy);
    }

    public void EquipItem(Equipment equipment)
    {
        if (equippedItems.ContainsKey(equipment.equipmentType))
        {
            UnequipItem(equipment.equipmentType);
        }

        equippedItems[equipment.equipmentType] = equipment;
        ApplyStatBonuses(equipment);
    }

    public void UnequipItem(EquipmentType equipmentType)
    {
        if (equippedItems.ContainsKey(equipmentType))
        {
            Equipment equipment = equippedItems[equipmentType];
            RemoveStatBonuses(equipment);
            equippedItems.Remove(equipmentType);
        }
    }

    private void ApplyStatBonuses(Equipment equipment)
    {
        attackPower += equipment.attackBonus;
        defensePower += equipment.defenseBonus;
    }

    private void RemoveStatBonuses(Equipment equipment)
    {
        attackPower -= equipment.attackBonus;
        defensePower -= equipment.defenseBonus;
    }
}
