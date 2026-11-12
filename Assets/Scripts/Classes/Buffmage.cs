using System.Collections.Generic;
using UnityEngine;

public sealed class Buffmage : Companion
{
    public const string HeroID = "Buffmage";
    public BuffmageSettings settings;
    static readonly SkillType[] SkillTypes = { SkillType.BuffmageFireball, SkillType.BuffmageMagicMissile, SkillType.BuffmageMeteor };
    public override List<SkillType> AllSkills => new List<SkillType>(SkillTypes);
    public override List<SkillType> MainSkills => AllSkills;

    protected override void Awake()
    {
        if (settings == null) settings = BuffmageSettings.Load();
        if (GameManager.Instance != null && GameManager.Instance.characterDataDictionary.TryGetValue(HeroID, out var data))
            characterData = data;
        else if (characterData != null) characterData = Instantiate(characterData);
        base.Awake();
        characterIDNumber = HeroID;
        heroIcon = characterData.heroIcon; fullHeroImage = characterData.fullHeroImage;
        if (settings != null) { hitSound = settings.hitSound; criticalHitSound = settings.perfectSound; }
        InitializeSkillsBasedOnLevel();
        var driver = GetComponent<BuffmageAnimationDriver>() ?? gameObject.AddComponent<BuffmageAnimationDriver>();
        driver.Initialize(settings != null ? settings.animations : null);
        if (GetComponent<BuffmagePresentation>() == null) gameObject.AddComponent<BuffmagePresentation>();
    }

    public static void InitializeNewData(CharacterData data, BuffmageSettings config)
    {
        if (config == null) return;
        data.heroLevel = 1; data.heroExp = 0;
        data.maxHealth = data.health = config.startingHealth;
        data.attackPower = config.startingAttack; data.defensePower = config.startingDefense;
        data.speed = config.startingSpeed;
        data.maxEnergy = data.energy = config.startingEnergy;
        data.isUnlocked = false;
    }

    public override void InitializeSkillsBasedOnLevel()
    {
        availableSkills.Clear(); availableSkills.AddRange(SkillTypes);
        if (characterData == null) return;
        characterData.AvailableSkills = new List<SkillType>(availableSkills);
        characterData.LockedSkills = new List<SkillType>();
        if (characterData.equippedSkills == null || characterData.equippedSkills.Count == 0)
            characterData.equippedSkills = new List<SkillType>(SkillTypes);
        ValidateEquippedSkills();
    }

    public override Skill GetSkillInstance(SkillType type) => BuffmageSkills.Create(type, settings);

    public override void CheckForDeath()
    {
        if (health <= 0 && GetComponent<BuffmageAnimationDriver>()?.TryPlayDeath() == true) return;
        base.CheckForDeath();
    }

    public override void LevelUp()
    {
        if (settings == null) return;
        while (heroExp >= ExpToNextLevel(heroLevel))
        {
            heroExp -= ExpToNextLevel(heroLevel);
            int previous = heroLevel++;
            // Incremental growth preserves saved gear bonuses and purchased stats.
            maxHealth += settings.healthPerLevel; health = Mathf.Min(maxHealth, health + settings.healthPerLevel);
            attackPower += settings.attackPerLevel;
            defensePower += Mathf.FloorToInt((heroLevel - 1) * settings.defensePerLevel)
                - Mathf.FloorToInt((previous - 1) * settings.defensePerLevel);
            if (settings.energyEveryLevels > 0 && heroLevel % settings.energyEveryLevels == 0) maxEnergy++;
            energy = maxEnergy; heroStatPoints++; heroSkillPoints++;
        }
        InitializeSkillsBasedOnLevel(); SaveCharacterData();
    }
}
