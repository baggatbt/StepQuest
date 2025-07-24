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

    private const int BASE_ENERGY = 2;

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
    int lvl = heroLevel;  // use the Companion’s heroLevel, not Character.level

    maxHealth   = BASE_HEALTH + (lvl - 1) * HEALTH_GROWTH;
    health      = maxHealth;

    attackPower = BASE_ATTACK + (lvl - 1) * ATTACK_GROWTH;

    // force float division so your growth works correctly
    speed       = BASE_SPEED + Mathf.FloorToInt((lvl - 1) / 3f);
    defensePower= BASE_DEFENSE + Mathf.FloorToInt((lvl - 1) / 4f);

    maxEnergy   = BASE_ENERGY + Mathf.FloorToInt((lvl - 1) / 2f);
    energy      = maxEnergy;
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
