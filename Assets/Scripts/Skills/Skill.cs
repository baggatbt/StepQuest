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
    public float skillDamageModifier = 1f; // keep: extra multiplier per skill

    // ----------------------------
    // NEW: Power + Speed Framework
    // ----------------------------

    [Header("Power + Speed")]
    [Tooltip("How hard the skill hits. Think 'move power' like Pokémon/Nexomon. Typical: 30-120.")]
    public int power = 60;

    [Tooltip("How fast the skill resolves relative to user's Speed. >1 = faster, <1 = slower.")]
    public float speedMultiplier = 1.0f;

    [Tooltip("Optional: breaks ties / nudges ordering. Higher priority goes first when very close.")]
    public int priority = 0;

    [Header("Variance (Optional)")]
    [Tooltip("If enabled, applies a small random multiplier to damage AFTER all other multipliers.")]
    public bool useVariance = false;

    [Tooltip("Pokémon-style is 0.85–1.00 (only downward). For gentler, use 0.90–1.00.")]
    [Range(0.5f, 1f)] public float varianceMin = 0.90f;

    [Range(1f, 1.5f)] public float varianceMax = 1.00f;

    // Section C additions
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
        // Default implementation can be empty
    }

    // ==========================================================
    // NEW HELPERS: Speed + Power plumbing (BattleManager uses these)
    // ==========================================================

    /// <summary>
    /// Given a user's raw Speed stat (int), returns the effective speed for this skill.
    /// BattleManager can use this for turn scheduling / action gauges.
    /// </summary>
    public virtual int GetEffectiveSpeed(int userSpeed)
    {
        // Clamp so a slow skill can't go to 0 speed.
        return Mathf.Max(1, Mathf.RoundToInt(userSpeed * speedMultiplier));
    }

    /// <summary>
    /// Optional: produce a "time cost" value from effective speed.
    /// Lower time cost = acts sooner. Use whichever convention your timeline uses.
    /// </summary>
    public virtual float GetActionTimeCost(int userSpeed, float baseTimeCost = 100f)
    {
        int eff = GetEffectiveSpeed(userSpeed);
        // Example convention: time cost shrinks as speed rises.
        return baseTimeCost / eff;
    }

    // ==========================================================
    // NEW HELPERS: Damage calculation using Power (clean + readable)
    // ==========================================================

    /// <summary>
    /// A simple, readable damage model:
    /// BaseDamage ≈ (Attack / Defense) * Power * LevelFactor * skillDamageModifier
    /// This keeps numbers small and intuitive.
    ///
    /// You can call this from each skill if you have atk/def/level available.
    /// </summary>
    protected virtual int CalculateBaseDamage(int userAttack, int targetDefense, int userLevel)
    {
        // Safe guards
        float atk = Mathf.Max(1, userAttack);
        float def = Mathf.Max(1, targetDefense);

        // Gentle level scaling that won't explode numbers:
        // Level 1 -> 1.00, Level 50 -> 1.49, Level 100 -> 1.99
        float levelFactor = 1.0f + (Mathf.Clamp(userLevel, 1, 100) - 1) * 0.01f;

        float ratio = atk / def;
        float raw = ratio * power * levelFactor * skillDamageModifier;

        // Keep small numbers readable:
        int dmg = Mathf.Max(1, Mathf.RoundToInt(raw * 0.10f)); // 0.10f is your global tuning knob

        if (useVariance)
            dmg = ApplyVariance(dmg);

        return dmg;
    }

    /// <summary>
    /// Keep your old hook for skills that still want "just return 5" etc.
    /// If your derived skills override this, nothing breaks.
    /// </summary>
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

    // ==========================================================
    // Timing multipliers (keeps your existing behavior)
    // ==========================================================

    protected virtual float GetPlayerTimingDamageMultiplier(TimingEventResult timingResult)
        => (timingResult == TimingEventResult.Good) ? 1.25f : 1.0f;

    protected virtual float GetEnemyTimingDamageMultiplier(TimingEventResult timingResult)
        => (timingResult == TimingEventResult.Good) ? 0.75f : 1.0f;

    // ==========================================================
    // YOUR EXISTING HANDLERS (kept, lightly centralized)
    // ==========================================================

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