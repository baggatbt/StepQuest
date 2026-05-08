using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Knight : Companion
{
    private const int BASE_HEALTH = 12;
    private const int HEALTH_GROWTH = 3;

    private const int BASE_ATTACK = 2;
    private const float ATTACK_GROWTH_FACTOR = 0.5f;

    private const int BASE_SPEED = 4;

    protected override void Awake()
    {
        base.Awake();

        skillOne = SkillType.Slash;
        skillTwo = SkillType.TripleHit;

        InitializeSkillsBasedOnLevel();
        UpdateStats();
        SaveCharacterData();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();

        // Level 1
        availableSkills.Add(SkillType.Slash);

        // Level 2 unlock
        if (heroLevel >= 2)
        {
            availableSkills.Add(SkillType.TripleHit);

            // Option A: auto-equip when unlocked
            if (characterData != null && !characterData.equippedSkills.Contains(SkillType.TripleHit))
            {
                characterData.equippedSkills.Add(SkillType.TripleHit);
                Debug.Log("[Knight] TripleHit unlocked and auto-equipped.");
            }
        }

        ValidateEquippedSkills();

        Debug.Log($"[Knight] Level {heroLevel} Available skills: {string.Join(", ", availableSkills)}");
        Debug.Log($"[Knight] Equipped skills: {string.Join(", ", characterData.equippedSkills)}");
    }

    public void UpdateStats()
    {
        maxHealth = BASE_HEALTH + (heroLevel - 1) * HEALTH_GROWTH;
        attackPower = BASE_ATTACK + Mathf.FloorToInt((heroLevel - 1) * ATTACK_GROWTH_FACTOR);
        speed = BASE_SPEED;

        if (health <= 0 || health > maxHealth)
            health = maxHealth;
    }

    public override void LevelUp()
    {
        while (heroExp >= ExpToNextLevel(heroLevel))
        {
            heroExp -= ExpToNextLevel(heroLevel);
            heroLevel++;

            UpdateStats();
            InitializeSkillsBasedOnLevel();

            energy = maxEnergy;

            heroStatPoints += 1;
            heroSkillPoints += 1;

            SaveCharacterData();

            Debug.Log($"[Knight] Leveled up to {heroLevel}");
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
        SkillType.SpeedBreak,
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