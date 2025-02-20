using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Knight : Companion
{
    // Constants for Knight's stat growth
    private const int BASE_HEALTH = 12;            // Above average starting health
    private const int HEALTH_GROWTH = 3;           // Gains 3 HP per level after level 1

    private const int BASE_ATTACK = 2;             // Average starting attack
    private const float ATTACK_GROWTH_FACTOR = 0.5f; // +1 attack every 2 levels

    private const int BASE_SPEED = 4;              // Below average speed

    protected override void Awake()
    {
        base.Awake();
        skillOne = SkillType.TripleHit;
        skillTwo = SkillType.Taunt;
        // Assign specific paths for Knight icons
        if (characterData != null)
        {
           // characterData.heroIconPath = "Assets/Resources/Sprites/GUI/knightIcon.png";
           // characterData.fullHeroImagePath = "Path/To/KnightFullImage";
        }
        InitializeSkillsBasedOnLevel();
        UpdateStats(); // Initialize stats on Awake
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();
        availableSkills.Add(SkillType.Slash);
        // Uncomment these as needed for level-based unlocking:
        // if (this.heroLevel >= 2) availableSkills.Add(SkillType.TripleHit);
        // if (this.heroLevel >= 5) availableSkills.Add(SkillType.Taunt);
        Debug.Log($"Total Available skills: {availableSkills.Count}");
    }

    public void UpdateStats()
    {
        // Update stats based on the current level using our growth formulas:
        this.maxHealth = BASE_HEALTH + (this.level - 1) * HEALTH_GROWTH;
        this.health = this.maxHealth;
        this.attackPower = BASE_ATTACK + Mathf.FloorToInt((this.level - 1) * ATTACK_GROWTH_FACTOR);
        this.speed = BASE_SPEED;
        // Optionally update energy or other stats if necessary
    }

     public override void LevelUp()
    {
        if (this.heroExp >= ExpToNextLevel(this.heroLevel))
        {
            this.heroExp -= ExpToNextLevel(this.heroLevel);
            this.heroLevel++;
            UpdateStats();
            // Do not reassign health here because UpdateStats already set health = maxHealth.
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
