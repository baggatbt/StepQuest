using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TamedGoblin : Companion
{
    private const int BASE_HEALTH = 12;
    private const int HEALTH_GROWTH = 3;
    private const int BASE_ATTACK = 6;
    private const float ATTACK_GROWTH_FACTOR = 1.5f;
    private const int BASE_SPEED = 7;

    protected override void Awake()
    {
        base.Awake();
        skillOne = SkillType.TripleHit;
        skillTwo = SkillType.None; // Add more if you want

        InitializeSkillsBasedOnLevel();
        UpdateStats();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();
        availableSkills.Add(SkillType.Slash);
    }

    public override void LevelUp()
    {
        if (this.heroExp >= ExpToNextLevel(this.heroLevel))
        {
            this.heroExp -= ExpToNextLevel(this.heroLevel);
            this.heroLevel++;
            UpdateStats();

            this.energy = this.maxEnergy;
            this.heroStatPoints += 1;
            this.heroSkillPoints += 1;

            Debug.Log("Tamed Goblin leveled up to level " + this.heroLevel);
        }
    }

    public override void UpdateStats()
    {
        this.maxHealth = BASE_HEALTH + (this.heroLevel - 1) * HEALTH_GROWTH;
        this.health = this.maxHealth;
        this.attackPower = BASE_ATTACK + Mathf.FloorToInt((this.heroLevel - 1) * ATTACK_GROWTH_FACTOR);
        this.speed = BASE_SPEED;
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
