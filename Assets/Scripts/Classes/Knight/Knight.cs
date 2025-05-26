using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Knight : Companion
{
    // Base stats and growth
    private const int BASE_HEALTH = 20;
    private const int HEALTH_GROWTH = 5;

    private const int BASE_ATTACK = 10;
    private const int ATTACK_GROWTH = 2;

    private const int BASE_SPEED = 4;

    private const int BASE_DEFENSE = 2;
    private const int DEFENSE_GROWTH = 1;

    private const int BASE_ENERGY = 3;

    protected override void Awake()
    {
        base.Awake();
        skillOne = SkillType.TripleHit;
        skillTwo = SkillType.Taunt;

        if (characterData != null)
        {
            // characterData.heroIconPath = "Assets/Resources/Sprites/GUI/knightIcon.png";
            // characterData.fullHeroImagePath = "Path/To/KnightFullImage";
        }

        InitializeSkillsBasedOnLevel();
        UpdateStats();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();
        availableSkills.Add(SkillType.Slash);
        Debug.Log($"Total Available skills: {availableSkills.Count}");
    }

    public void UpdateStats()
{
    heroLevel = Mathf.Max(heroLevel, 1); // safety

    this.maxHealth = BASE_HEALTH + (this.heroLevel - 1) * HEALTH_GROWTH;
    this.health = this.maxHealth;

    this.attackPower = BASE_ATTACK + (this.heroLevel - 1) * ATTACK_GROWTH;
    this.speed = BASE_SPEED + Mathf.FloorToInt((this.heroLevel - 1) / 3);
    this.defensePower = BASE_DEFENSE + Mathf.FloorToInt((this.heroLevel - 1) / 4);
    this.maxEnergy = BASE_ENERGY + Mathf.FloorToInt((this.heroLevel - 1) / 2);
    this.energy = this.maxEnergy;

    // Write these values back to characterData
    if (characterData != null)
    {
        characterData.maxHealth = maxHealth;
        characterData.health = health;
        characterData.attackPower = attackPower;
        characterData.speed = speed;
        characterData.defensePower = defensePower;
        characterData.maxEnergy = maxEnergy;
        characterData.energy = energy;
    }
}


    public override void LevelUp()
    {
        if (this.heroExp >= ExpToNextLevel(this.heroLevel))
        {
            this.heroExp -= ExpToNextLevel(this.heroLevel);
            this.heroLevel++;
            UpdateStats();
            this.heroStatPoints += 1;
            this.heroSkillPoints += 1;
            Debug.Log("Hero leveled up to level " + this.heroLevel);
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
