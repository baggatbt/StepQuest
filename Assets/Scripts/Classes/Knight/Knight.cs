using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Knight : Companion
{
    private const int BASE_HEALTH = 24;
    private const int HEALTH_GROWTH = 5;

    private const int BASE_ATTACK = 5;
    private const float ATTACK_GROWTH_FACTOR = 1.1f;

    private const int BASE_DEFENSE = 2;
    private const float DEFENSE_GROWTH_FACTOR = 0.5f;

private const int BASE_SPEED = 4;

    protected override void Awake()
    {
        base.Awake();

        skillOne = SkillType.Slash;
        
        

        InitializeSkillsBasedOnLevel();
        UpdateStats();
        SaveCharacterData();
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear();

        AddAvailableSkill(SkillType.Slash);
        

        // Level 3 unlock CHANGED TO DEFAULT FOR TESTING
        if (heroLevel >= 1)
        {
            AddAvailableSkill(SkillType.HeavySlash);
            TryAutoEquipSkill(SkillType.HeavySlash);
        }

        // Level 2 unlock
        if (heroLevel >= 2)
        {
            AddAvailableSkill(SkillType.TripleHit);
            TryAutoEquipSkill(SkillType.TripleHit);
        }

        ValidateEquippedSkills();

        if (characterData != null)
        {
            characterData.AvailableSkills = new List<SkillType>(availableSkills);
        }

        Debug.Log($"[Knight] Level {heroLevel} Available skills: {string.Join(", ", availableSkills)}");

        if (characterData != null)
            Debug.Log($"[Knight] Equipped skills: {string.Join(", ", characterData.equippedSkills)}");
    }

    private void AddAvailableSkill(SkillType skill)
    {
        if (!availableSkills.Contains(skill))
            availableSkills.Add(skill);
    }

    private void TryAutoEquipSkill(SkillType skill)
    {
        if (characterData == null)
            return;

        if (characterData.equippedSkills == null)
            characterData.equippedSkills = new List<SkillType>();

        if (characterData.equippedSkills.Contains(skill))
            return;

        int maxSlots = Mathf.Max(1, characterData.maxEquippedSkills);

        if (characterData.equippedSkills.Count < maxSlots)
        {
            characterData.equippedSkills.Add(skill);
            Debug.Log($"[Knight] {skill} unlocked and auto-equipped.");
        }
        else
        {
            Debug.Log($"[Knight] {skill} unlocked but not auto-equipped because skill slots are full.");
        }
    }

    public void UpdateStats()
{
    int previousMaxHealth = maxHealth;

    maxHealth = BASE_HEALTH + ((heroLevel - 1) * HEALTH_GROWTH);

    attackPower = BASE_ATTACK + Mathf.FloorToInt((heroLevel - 1) * ATTACK_GROWTH_FACTOR);

    defensePower = BASE_DEFENSE + Mathf.FloorToInt((heroLevel - 1) * DEFENSE_GROWTH_FACTOR);

    speed = BASE_SPEED;

    ApplyKnightPathStatBonuses();
    ApplyKnightPassiveStatBonuses();

    // If this is a brand-new character or health is invalid, fill health.
    if (health <= 0)
    {
        health = maxHealth;
    }
    // If max HP increased from leveling, add the difference to current HP.
    else if (maxHealth > previousMaxHealth && previousMaxHealth > 0)
    {
        int hpGained = maxHealth - previousMaxHealth;
        health += hpGained;
        health = Mathf.Clamp(health, 1, maxHealth);
    }
    // If max HP somehow went down, clamp current HP.
    else if (health > maxHealth)
    {
        health = maxHealth;
    }
}

    private void ApplyKnightPathStatBonuses()
    {
        if (characterData == null)
            return;

        switch (characterData.knightPath)
        {
            case KnightPath.Guardian:
                maxHealth += 5;
                defensePower += 1;
                break;

            case KnightPath.Duelist:
                attackPower += 1;
                speed += 1;
                break;

            case KnightPath.Spellblade:
                attackPower += 1;
                break;
        }
    }

    private void ApplyKnightPassiveStatBonuses()
    {
        if (characterData == null)
            return;

        switch (characterData.knightPassive)
        {
            case KnightPassive.IronBody:
                maxHealth = Mathf.RoundToInt(maxHealth * 1.10f);
                break;
        }
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

            CheckMilestoneUnlocks();

            SaveCharacterData();

            Debug.Log($"[Knight] Leveled up to {heroLevel}");
        }
    }

    private void CheckMilestoneUnlocks()
    {
        if (characterData == null)
            return;

        if (heroLevel == 3)
        {
            Debug.Log("[Knight] Milestone reached: Triple Slash unlocked!");
        }

        if (heroLevel == 5)
        {
            Debug.Log("[Knight] Milestone reached: Taunt unlocked!");
        }

        if (heroLevel >= 6 && !characterData.hasChosenLevel6Modifier)
        {
            Debug.Log("[Knight] Level 6 reached. Player should choose a Slash modifier.");
            // Later: open modifier choice UI here.
        }

        if (heroLevel >= 8 && !characterData.hasChosenLevel8Passive)
        {
            Debug.Log("[Knight] Level 8 reached. Player should choose a passive.");
            // Later: open passive choice UI here.
        }

        if (heroLevel >= 10 && !characterData.hasChosenLevel10Path)
        {
            Debug.Log("[Knight] Level 10 reached. Player should choose a Knight path.");
            // Later: open path choice UI here.
        }
    }

    public void ChooseSlashModifier(KnightSlashModifier modifier)
    {
        if (characterData == null)
            return;

        if (heroLevel < 6)
        {
            Debug.LogWarning("[Knight] Slash modifier requires level 6.");
            return;
        }

        if (characterData.hasChosenLevel6Modifier)
        {
            Debug.LogWarning("[Knight] Slash modifier already chosen.");
            return;
        }

        characterData.knightSlashModifier = modifier;
        characterData.hasChosenLevel6Modifier = true;

        SaveCharacterData();

        Debug.Log($"[Knight] Chose Slash modifier: {modifier}");
    }

    public void ChoosePassive(KnightPassive passive)
    {
        if (characterData == null)
            return;

        if (heroLevel < 8)
        {
            Debug.LogWarning("[Knight] Passive choice requires level 8.");
            return;
        }

        if (characterData.hasChosenLevel8Passive)
        {
            Debug.LogWarning("[Knight] Passive already chosen.");
            return;
        }

        characterData.knightPassive = passive;
        characterData.hasChosenLevel8Passive = true;

        UpdateStats();
        SaveCharacterData();

        Debug.Log($"[Knight] Chose passive: {passive}");
    }

    public void ChoosePath(KnightPath path)
    {
        if (characterData == null)
            return;

        if (heroLevel < 10)
        {
            Debug.LogWarning("[Knight] Path choice requires level 10.");
            return;
        }

        if (characterData.hasChosenLevel10Path)
        {
            Debug.LogWarning("[Knight] Path already chosen.");
            return;
        }

        characterData.knightPath = path;
        characterData.hasChosenLevel10Path = true;

        UpdateStats();
        SaveCharacterData();

        Debug.Log($"[Knight] Chose path: {path}");
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
    SkillType.HeavySlash,
    SkillType.TripleHit,
    SkillType.Taunt,
    SkillType.SpeedBreak,
};

    private List<SkillType> mainSkills = new List<SkillType>
{
    SkillType.Slash,
    SkillType.HeavySlash,
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
                
            case SkillType.HeavySlash:
                return new HeavySlash();

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