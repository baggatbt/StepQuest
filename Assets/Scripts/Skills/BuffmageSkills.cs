using System.Collections;
using UnityEngine;

public static class BuffmageSkills
{
    public static Skill Create(SkillType type, BuffmageSettings settings = null)
    {
        if (settings == null) settings = BuffmageSettings.Load();
        if (settings == null) { Debug.LogError("Buffmage settings asset is missing."); return null; }
        return type switch {
            SkillType.BuffmageFireball => new BuffmageFireball(settings),
            SkillType.BuffmageMagicMissile => new BuffmageMagicMissile(settings),
            SkillType.BuffmageMeteor => new BuffmageMeteor(settings),
            _ => null
        };
    }
}

public abstract class BuffmageSkill : Skill
{
    protected readonly BuffmageSettings config;
    protected string interactionHint;
    protected BuffmageSkill(BuffmageSettings settings)
    {
        config = settings; requiresMovement = false; noZoom = true;
        skillLevel = requiredLevel = 1; damageVarianceMin = damageVarianceMax = 1;
    }
    protected bool Alive(Character user, Character target, BattleManager battle)
        => user != null && target != null && target.health > 0 && battle != null && battle.CanResolveAction(user);
    protected BuffmagePresentation Begin(Character user, Character target)
    {
        skillExecutionComplete = false; user.isAttacking = true; user.damageApplied = false;
        var presentation = user.GetComponent<BuffmagePresentation>() ?? user.gameObject.AddComponent<BuffmagePresentation>();
        presentation.Begin(config, target != null ? target.transform : null); return presentation;
    }
    protected void Finish(Character user, BattleManager battle)
    {
        battle?.CancelTimedAction();
        if (user != null)
        {
            user.GetComponent<BuffmagePresentation>()?.ResetPresentation();
            user.isAttacking = false; user.isAnimationDone = false; user.damageApplied = false;
        }
        skillExecutionComplete = true;
    }
    protected void Hit(Character user, Character target, int damage, TimingEventResult timing, BattleManager battle = null)
    {
        RecordTiming(timing); target.TakeDamage(Mathf.Max(1, damage), user);
        if (timing == TimingEventResult.Good) user.PlayCriticalHitSound(); else user.PlayHitSound();
        if (battle != null && battle.cameraShake != null) battle.CameraShakeMagnitude(timing);
    }
    public override string GetBattlePreviewText(Character user)
        => interactionHint + "\n" + base.GetBattlePreviewText(user) + "   CD: " + cooldownTurns;
}

public sealed class BuffmageFireball : BuffmageSkill
{
    public override Vector2Int GetBaseDamageRange(Character user)
    {
        float attack = user != null ? user.attackPower * damageMultiplier : 0;
        return new Vector2Int(Mathf.Max(1, Mathf.RoundToInt(attack * ActionTimingRules.ChargeMultiplier(0, config.fireballCharge))),
            Mathf.Max(1, Mathf.RoundToInt(attack * config.fireballCharge.perfectMultiplier)));
    }
    public BuffmageFireball(BuffmageSettings settings) : base(settings)
    {
        Type = SkillType.BuffmageFireball; skillName = "Fireball?";
        interactionHint = "HOLD, then release in gold";
        description = "Who needs magic when you can set a book on fire and throw it really hard? Select a target, then HOLD and release in gold.";
        iconImage = settings.fireballIcon; damageMultiplier = settings.fireballDamage; speedMultiplier = settings.fireballSpeed;
        energyCost = settings.fireballEnergyCost; energyGain = settings.fireballEnergyGain; cooldownTurns = settings.fireballCooldown;
    }
    public override IEnumerator Execute(Character user, Character target, BattleManager battle)
    {
        try
        {
            if (!Alive(user, target, battle)) yield break;
            var visual = Begin(user, target); visual.PrepareBook();
            TimedActionResult timing = default;
            yield return battle.PlayerHoldReleaseTimeEvent(user, config.fireballCharge, value => timing = value, visual.ChargeBook);
            if (timing.cancelled || !Alive(user, target, battle)) yield break;
            string message = timing.grade == TimingEventResult.Good ? "PERFECT RELEASE!"
                : timing.grade == TimingEventResult.Late ? "OVERCHARGED!" : "EARLY RELEASE";
            battle.ReportTimedFeedback(message, timing.grade);
            yield return visual.ThrowBook(target.transform, timing.grade == TimingEventResult.Late);
            if (!Alive(user, target, battle)) yield break;
            int damage = Mathf.RoundToInt(user.attackPower * damageMultiplier * ActionTimingRules.ChargeMultiplier(timing.seconds, config.fireballCharge));
            Hit(user, target, damage, timing.grade, battle); visual.Impact(target.transform, timing.grade == TimingEventResult.Good);
            user.GainEnergy(energyGain); target.CheckForDeath();
            yield return new WaitForSecondsRealtime(.3f);
        }
        finally { Finish(user, battle); }
    }
}

public sealed class BuffmageMagicMissile : BuffmageSkill
{
    public override Vector2Int GetBaseDamageRange(Character user)
        => new Vector2Int(0, Mathf.Clamp(config.maximumPunches, 1, 30) * Mathf.Max(1,
            Mathf.RoundToInt((user != null ? user.attackPower : 0) * config.punchDamage)));
    public BuffmageMagicMissile(BuffmageSettings settings) : base(settings)
    {
        Type = SkillType.BuffmageMagicMissile; skillName = "Magic Missile";
        interactionHint = "TAP repeatedly when the attack begins";
        description = "An unstoppable barrage of extremely non-magical projectiles. TAP repeatedly when the attack begins.";
        iconImage = settings.punchIcon; damageMultiplier = settings.punchDamage * settings.maximumPunches;
        speedMultiplier = 1.15f; energyCost = settings.barrageEnergyCost; cooldownTurns = settings.barrageCooldown;
    }
    public override IEnumerator Execute(Character user, Character target, BattleManager battle)
    {
        try
        {
            if (!Alive(user, target, battle)) yield break;
            var visual = Begin(user, target); TimedActionResult timing = default;
            int landedHits = 0;
            yield return battle.PlayerRapidInputEvent(user, config.barrageSeconds, config.maximumPunches, config.minimumPunchInterval,
                count => {
                    if (!Alive(user, target, battle)) return false;
                    visual.Punch(target.transform, count, () => {
                        if (!Alive(user, target, battle)) return;
                        Hit(user, target, Mathf.RoundToInt(user.attackPower * config.punchDamage), TimingEventResult.Early);
                        visual.Impact(target.transform, false); landedHits++;
                    });
                    return target.health > 0;
                }, value => timing = value);
            if (timing.cancelled) yield break;
            yield return visual.WaitForPunches();
            RecordTiming(timing.grade);
            battle.ReportTimedFeedback(landedHits == 0 ? "MISS - NO PUNCHES" : landedHits + " HITS! VERY MAGICAL.", timing.grade);
            if (timing.grade == TimingEventResult.Good) user.PlayCriticalHitSound();
            target.CheckForDeath(); yield return new WaitForSecondsRealtime(.3f);
        }
        finally { Finish(user, battle); }
    }
}

public sealed class BuffmageMeteor : BuffmageSkill
{
    public override Vector2Int GetBaseDamageRange(Character user)
    {
        float attack = user != null ? user.attackPower * damageMultiplier : 0;
        return new Vector2Int(Mathf.Max(1, Mathf.RoundToInt(attack * config.meteorMissMultiplier)),
            Mathf.Max(1, Mathf.RoundToInt(attack * config.meteorPerfectMultiplier)));
    }
    public BuffmageMeteor(BuffmageSettings settings) : base(settings)
    {
        Type = SkillType.BuffmageMeteor; skillName = "Meteor";
        interactionHint = "TAP when the marker reaches gold";
        description = "If you can't summon a meteor, become one. TAP when the marker reaches gold.";
        iconImage = settings.meteorIcon; damageMultiplier = settings.meteorDamage;
        speedMultiplier = .65f; energyCost = settings.meteorEnergyCost; cooldownTurns = settings.meteorCooldown;
    }
    public override IEnumerator Execute(Character user, Character target, BattleManager battle)
    {
        try
        {
            if (!Alive(user, target, battle)) yield break;
            var visual = Begin(user, target); TimedActionResult timing = default;
            visual.PrepareMeteor();
            yield return battle.PlayerActiveTimeEvent(user, config.meteorTiming, value => timing = value);
            if (timing.cancelled || !Alive(user, target, battle)) yield break;
            bool perfect = timing.grade == TimingEventResult.Good;
            battle.ReportTimedFeedback(perfect ? "PERFECT METEOR!" : "UNSCHEDULED LANDING!", timing.grade);
            yield return visual.LeapOnto(target.transform);
            if (!Alive(user, target, battle)) yield break;
            float multiplier = perfect ? config.meteorPerfectMultiplier : config.meteorMissMultiplier;
            Hit(user, target, Mathf.RoundToInt(user.attackPower * damageMultiplier * multiplier), timing.grade, battle);
            visual.Impact(target.transform, perfect); target.CheckForDeath();
            yield return visual.LandAndRecover();
        }
        finally { Finish(user, battle); }
    }
}
