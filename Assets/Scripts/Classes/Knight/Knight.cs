using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Knight : Companion
{
    // Base stats and growth
    private const int BASE_HEALTH = 18;
    private const int HEALTH_GROWTH = 5;

    private const int BASE_ATTACK = 7;
    private const int ATTACK_GROWTH = 2;

    private const int BASE_SPEED = 4;

    private const int BASE_DEFENSE = 1;
    private const int DEFENSE_GROWTH = 1;

    private const int BASE_ENERGY = 5;

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
    int lvl = Mathf.Max(heroLevel, 1);

    maxHealth   = BASE_HEALTH + (lvl - 1) * HEALTH_GROWTH;
    //health      = maxHealth;

    attackPower = BASE_ATTACK + (lvl - 1) * ATTACK_GROWTH;
    speed       = BASE_SPEED + Mathf.FloorToInt((lvl - 1) / 3f);
    defensePower= BASE_DEFENSE + Mathf.FloorToInt((lvl - 1) / 4f);
    maxEnergy   = BASE_ENERGY + Mathf.FloorToInt((lvl - 1) / 2f);
    //energy      = maxEnergy;

    // Sync back to the characterData if the UI or inspector reads from it
    if (characterData != null)
    {
        characterData.maxHealth = maxHealth;
        characterData.health = health;
        characterData.attackPower = attackPower;
        characterData.defensePower = defensePower;
        characterData.speed = speed;
        characterData.maxEnergy = maxEnergy;
        characterData.energy = energy;
        characterData.heroLevel = heroLevel;
        characterData.heroStatPoints = heroStatPoints;
        characterData.heroSkillPoints = heroSkillPoints;
    }
    SaveCharacterData();
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
        SkillType.SwordWave
    };

    private List<SkillType> mainSkills = new List<SkillType>
    {
        SkillType.Slash,
        SkillType.TripleHit,
        SkillType.Taunt,
        SkillType.SpeedBreak,
        SkillType.SwordWave
    };

    public override List<SkillType> MainSkills => mainSkills;

    public override Skill GetSkillInstance(SkillType skillType)
{
    switch (skillType)
    {
        case SkillType.Slash:
        {
            var s = new Slash();
            s.Type = SkillType.Slash;
            return s;
        }
        case SkillType.TripleHit:
        {
            var s = new TripleHitSkill();
            s.Type = SkillType.TripleHit;
            return s;
        }
        case SkillType.Taunt:
        {
            var s = new Taunt();
            s.Type = SkillType.Taunt;
            return s;
        }
        case SkillType.ReflectDamagePassive:
        {
            var s = new ReflectDamagePassive();
            s.Type = SkillType.ReflectDamagePassive;
            return s;
        }
        case SkillType.SpeedBreak:
        {
            var s = new SpeedBreak();
            s.Type = SkillType.SpeedBreak;
            return s;
        }
        case SkillType.SwordWave:
        {
            var s = new SwordWave();
            s.Type = SkillType.SwordWave;
            return s;
        }
        default:
            Debug.LogError("Unknown skill type for Knight: " + skillType);
            return null;
    }
}

}
