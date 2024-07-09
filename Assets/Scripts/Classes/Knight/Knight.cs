using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Knight : Companion
{
    protected override void Awake()
    {
        base.Awake();
        
        // Load the skill tree prefab from the Resources folder (Assuming this step is required and has a method)
        //LoadSkillTree();

        // Initialize with default values if no data is loaded
        if (this.heroLevel == 0)
        {
            this.attackPower = 5;
            this.maxHealth = 12;
            this.health = this.maxHealth;
            this.defensePower = 1;
            this.speed = 4;
            this.maxEnergy = 5;
            this.energy = maxEnergy;
            this.defensePenetration = 0;
            this.heroID = "Knight";
            this.heroLevel = 1;
            this.heroExp = 0;
            this.heroSkillPoints = 0;
            this.heroStatPoints = 0;
            this.characterIDNumber = "1";
            this.maxStamina = 10;
            this.stamina = this.maxStamina; // Ensure stamina is initialized
        }
        else
        {
            LoadCharacterData(); // Load saved data
        }

        InitializeSkillsBasedOnLevel();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();
        availableSkills.Add(SkillType.Slash);
        if (this.heroLevel >= 2) availableSkills.Add(SkillType.TripleHit);
        if (this.heroLevel >= 5) availableSkills.Add(SkillType.Taunt);
        Debug.Log($"Total Available skills: {availableSkills.Count}");
    }

    public override void LevelUp()
    {
        if (this.heroExp >= ExpToNextLevel(this.heroLevel))
        {
            this.heroExp -= ExpToNextLevel(this.heroLevel);
            this.heroLevel++;

            switch (this.heroLevel)
            {
                case 2:
                    this.maxHealth += 2;
                    Debug.Log("Your max health went up by 2!");
                    break;
                case 3:
                    this.attackPower += 1;
                    break;
                case 4:
                    this.maxHealth += 3;
                    break;
                case 5:
                    this.attackPower += 1;
                    break;
                case 6:
                    this.maxEnergy += 1;
                    break;
                case 7:
                    this.maxHealth += 3;
                    break;
                case 8:
                    this.attackPower += 1;
                    break;
                case 9:
                    this.defensePower += 1;
                    break;
                case 10:
                    this.maxHealth += 4;
                    break;
                default:
                    break; // Extend this later when you have concrete balance plans
            }

            this.health = this.maxHealth;
            this.energy = this.maxEnergy;

            this.heroStatPoints += 1;
            this.heroSkillPoints += 1;

            Debug.Log("Hero leveled up to level " + this.heroLevel + ", stat upgraded.");
        }
    }

    public override List<SkillType> AvailableSkills => availableSkills;

    public override List<SkillType> LockedSkills => new List<SkillType>
    {
        SkillType.ReflectDamagePassive,
        SkillType.SpeedBreak,
    };

    public override List<SkillType> AllSkills => new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.SpeedBreak,
    };

    private List<SkillType> mainSkills = new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.SpeedBreak
    };

    public override List<SkillType> MainSkills => mainSkills;

    public override Skill GetSkillInstance(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Slash:
                return new Slash();
            case SkillType.TripleHit:
                return new TripleHitSkill();
            case SkillType.Taunt:
                return new Taunt();
            case SkillType.ReflectDamagePassive:
                return new ReflectDamagePassive();
            case SkillType.SpeedBreak:
                return new SpeedBreak();
            default:
                Debug.LogError("Unknown skill type for Knight: " + skillType);
                return null;
        }
    }
}
