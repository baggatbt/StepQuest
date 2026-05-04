using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackStage
{
    public string animationTrigger;
    public float timingWindowStart;
    public float timingWindowEnd;
    public int damage;
}

public enum SkillType
{
    None,
    Slash,
    TripleHit,
    SwordWave,
    GuardSkill,
    ShieldSlam,
    SlimeCompanionBasicAttack,
    WizardBasicAttack,
    FirePillar,
    Taunt,
    ShootArrow,
    ArrowRain,
    MeleeCombo,
    ReflectDamagePassive,
    SpeedBreak,
    TamedGoblinAttackSkill
}

public abstract class Skill
{
    [Header("Info")]
    public string skillName;
    public string description;

    [Header("Execution")]
    public bool skillExecutionComplete;
    public bool requiresMovement;
    public bool noZoom;
    public bool isActiveSkill = true;
    public int requiredLevel;
    public int skillLevel;
    public int skillPointCost;

    [Header("Energy")]
    public int energyCost;
    public int energyGain;
    public int energyGainBonus;
    public bool energyGained;
    public bool bonusGained;

    [Header("UI")]
    public Sprite iconImage;

    [Header("Damage")]
    public float skillDamageModifier = 1f;

    // Old power/speed fields kept so your other skills do not break
    public int power = 60;
    public float speedMultiplier = 1f;
    public int priority = 0;
    public bool useVariance = false;
    public float varianceMin = 0.9f;
    public float varianceMax = 1.1f;

    // New ATK-based preview/damage fields
    [Header("Damage Preview")]
    public float damageMultiplier = 1.0f;
    public float damageVarianceMin = 1.0f;
    public float damageVarianceMax = 1.2f;

    public SkillType Type { get; set; }
    public TimingEventResult LastTimingResult { get; private set; }

    protected TimingEventResult result;

    public Sprite LoadIconImage(string path)
    {
        return Resources.Load<Sprite>(path);
    }

    public abstract IEnumerator Execute(Character user, Character target, BattleManager battleManager);

    public virtual void ApplyPassiveEffect(CharacterData characterData)
    {
    }

    public virtual int GetEffectiveSpeed(int userSpeed)
    {
        return Mathf.Max(1, Mathf.RoundToInt(userSpeed * speedMultiplier));
    }

    public virtual float GetActionTimeCost(int userSpeed, float baseTimeCost = 100f)
    {
        int eff = GetEffectiveSpeed(userSpeed);
        return baseTimeCost / eff;
    }

    public virtual Vector2Int GetBaseDamageRange(Character user)
    {
        if (user == null) return new Vector2Int(0, 0);

        int min = Mathf.Max(1, Mathf.RoundToInt(user.attackPower * damageMultiplier * damageVarianceMin));
        int max = Mathf.Max(min, Mathf.RoundToInt(user.attackPower * damageMultiplier * damageVarianceMax));

        return new Vector2Int(min, max);
    }

    protected virtual int RollBaseDamage(Character user)
    {
        Vector2Int range = GetBaseDamageRange(user);
        return UnityEngine.Random.Range(range.x, range.y + 1);
    }

    public virtual string GetBattlePreviewText(Character user)
    {
        Vector2Int dmgRange = GetBaseDamageRange(user);
        int finalSpeed = user != null ? GetEffectiveSpeed(user.speed) : 0;

        return $"MP: {energyCost}   SPD: {finalSpeed}   DMG: {dmgRange.x}-{dmgRange.y}";
    }

    protected virtual int CalculateBaseDamage(int userAttack, int targetDefense, int userLevel)
    {
        float atk = Mathf.Max(1, userAttack);
        float levelFactor = 1.0f + (Mathf.Clamp(userLevel, 1, 100) - 1) * 0.01f;

        const float globalScalar = 0.03f;
        float raw = atk * power * levelFactor * skillDamageModifier * globalScalar;

        int dmg = Mathf.Max(1, Mathf.RoundToInt(raw));

        if (useVariance)
            dmg = ApplyVariance(dmg);

        return dmg;
    }

    protected virtual int CalculateBaseDamage(Character user)
    {
        return 5;
    }

    protected int ApplyVariance(int damage)
    {
        if (damage <= 1) return damage;

        float min = Mathf.Clamp(varianceMin, 0.5f, 1f);
        float max = Mathf.Max(min, varianceMax);

        float roll = UnityEngine.Random.Range(min, max);
        return Mathf.Max(1, Mathf.RoundToInt(damage * roll));
    }

    protected virtual float GetPlayerTimingDamageMultiplier(TimingEventResult timingResult)
        => (timingResult == TimingEventResult.Good) ? 1.25f : 1.0f;

    protected virtual float GetEnemyTimingDamageMultiplier(TimingEventResult timingResult)
        => (timingResult == TimingEventResult.Good) ? 0.75f : 1.0f;

    public void HandleAoeAttack(Character user, List<Character> enemies, TimingEventResult timingResult, int baseDamage)
    {
        result = timingResult;
        LastTimingResult = timingResult;

        float mult = GetPlayerTimingDamageMultiplier(timingResult);
        int dmg = Mathf.Max(1, Mathf.RoundToInt(baseDamage * mult));
        if (useVariance) dmg = ApplyVariance(dmg);

        foreach (Character target in enemies)
        {
            if (timingResult == TimingEventResult.Good)
            {
                target.animator.SetTrigger("IsHurtTrigger");
                user.PlayCriticalHitSound();
            }

            target.TakeDamage(dmg, user);
        }
    }

    public void HandleTimingResultForEnemyAttack(Character user, Character target, TimingEventResult timingResult, int baseDamage)
    {
        if (user.damageApplied) return;

        result = timingResult;
        LastTimingResult = timingResult;

        if (timingResult == TimingEventResult.Good)
        {
            target.didBlock = true;

            float mult = GetEnemyTimingDamageMultiplier(timingResult);
            int finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * mult));
            if (useVariance) finalDamage = ApplyVariance(finalDamage);

            target.TakeDamage(finalDamage, user);

            target.animator.SetTrigger("BlockTrigger");
            AudioManager.instance.PlayBlockSound();

            int finalReflectedDamage = CalculateReflectDamage(target, finalDamage);
            if (finalReflectedDamage > 0)
                user.TakeDamage(finalReflectedDamage, target);

            target.didBlock = false;
        }
        else
        {
            target.didBlock = false;

            int finalDamage = Mathf.Max(1, baseDamage);
            if (useVariance) finalDamage = ApplyVariance(finalDamage);

            target.TakeDamage(finalDamage, user);
            user.PlayHitSound();
        }

        user.damageApplied = true;
    }

    private int CalculateReflectDamage(Character user, int damage)
    {
        double reflectedDamage = user.damageReflectionPercentage * damage;
        int finalReflectedDamage = Convert.ToInt32(Math.Round(reflectedDamage));
        return finalReflectedDamage;
    }

    public void HandleTimingResultForPlayerAttack(Character user, Character target, TimingEventResult timingResult, int baseDamage)
    {
        if (user.damageApplied) return;

        result = timingResult;
        LastTimingResult = timingResult;

        float mult = GetPlayerTimingDamageMultiplier(timingResult);

        int finalDamage = Mathf.Max(1, Mathf.CeilToInt(baseDamage * mult));
        if (useVariance) finalDamage = ApplyVariance(finalDamage);

        target.TakeDamage(finalDamage, user);

        if (timingResult == TimingEventResult.Good)
        {
            if (!bonusGained)
            {
                bonusGained = true;
                user.GainEnergy(energyGainBonus);
            }

            user.PlayCriticalHitSound();
        }
        else
        {
            user.PlayHitSound();
        }

        user.damageApplied = true;
    }

    public void HandlePlayerRangedAttack(Character user, Projectile projectile, TimingEventResult timingResult)
    {
        this.result = timingResult;
        LastTimingResult = timingResult;

        float mult = GetPlayerTimingDamageMultiplier(timingResult);
        int dmg = Mathf.Max(1, Mathf.CeilToInt(projectile.damage * mult));
        if (useVariance) dmg = ApplyVariance(dmg);

        projectile.damage = dmg;
        user.PlayHitSound();
    }
}