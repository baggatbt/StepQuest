using System.Collections;
using UnityEngine;

public class HeavySlash : Skill
{
    private readonly float chargeStart = 0f;
    private readonly float chargeEnd = 1.25f;

    private const string ChargeBool = "isChargingHeavySlash";
    private const string ReleaseTrigger = "HeavySlashReleaseTrigger";

    public HeavySlash()
    {
        Type = SkillType.HeavySlash;

        skillName = "Heavy Slash";
        description = "Hold and release at the right time for a powerful single-hit slash. Stronger against defensive enemies.";

        energyCost = 0;
        energyGain = 1;
        energyGainBonus = 1;

        skillLevel = 1;
        requiresMovement = true;
        skillExecutionComplete = false;

        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Slash");

        // Higher than Slash because this is one single hit.
        // With Knight ATK 5, this previews around 8-9 damage before defense.
        damageMultiplier = 1.60f;
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.15f;

        // Heavy skill should be slower than basic Slash.
        speedMultiplier = 0.80f;
        priority = 0;

        useVariance = false;
    }

    public override string GetBattlePreviewText(Character user)
    {
        Vector2Int dmgRange = GetBaseDamageRange(user);
        int finalSpeed = user != null ? GetEffectiveSpeed(user.speed) : 0;

        return $"Hold Release\nMP: {energyCost}   SPD: {finalSpeed}   DMG: {dmgRange.x}-{dmgRange.y}";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        if (user == null || target == null)
            yield break;

        user.isAttacking = true;
        user.isAnimationDone = false;
        user.damageApplied = false;

        int baseDamage = RollBaseDamage(user);
        baseDamage = Mathf.Max(1, baseDamage);

        // Start the looping charge animation.
        if (user.animator != null)
        {
            user.animator.ResetTrigger(ReleaseTrigger);
            user.animator.SetBool(ChargeBool, true);
        }
        else
        {
            Debug.LogWarning("[HeavySlash] User has no animator.");
        }

        if (battleManager != null)
        {
            yield return battleManager.PlayerHoldReleaseTimeEvent(
                chargeStart,
                chargeEnd,
                (TimingEventResult result) =>
                {
                    // Stop the charge loop and play the actual slash animation.
                    if (user.animator != null)
                    {
                        user.animator.SetBool(ChargeBool, false);
                        user.animator.SetTrigger(ReleaseTrigger);
                    }

                    HandleTimingResultForPlayerAttack(user, target, result, baseDamage);

                    battleManager.CameraShakeMagnitude(result);
                    battleManager.ShowTimingResult(result.ToString());

                    if (result == TimingEventResult.Good)
                    {
                        Debug.Log("[HeavySlash] Perfect charge release.");
                    }
                });
        }
        else
        {
            // Fallback if somehow called without a BattleManager.
            if (user.animator != null)
            {
                user.animator.SetBool(ChargeBool, false);
                user.animator.SetTrigger(ReleaseTrigger);
            }

            target.TakeDamage(baseDamage, user);
            user.damageApplied = true;
        }

        // Wait for HeavySlashRelease animation event to call Character.AnimationEnded().
        if (user.animator != null)
        {
            yield return new WaitUntil(() => user.isAnimationDone);
        }
        else
        {
            yield return new WaitForSeconds(0.45f);
        }

        user.isAnimationDone = false;
        user.isAttacking = false;
        user.damageApplied = false;
        bonusGained = false;

        user.GainEnergy(energyGain);

        target.CheckForDeath();
    }

    protected override float GetPlayerTimingDamageMultiplier(TimingEventResult timingResult)
    {
        switch (timingResult)
        {
            case TimingEventResult.Good:
                return 1.50f;

            case TimingEventResult.Early:
                return 0.90f;

            case TimingEventResult.Late:
                return 0.90f;

            default:
                return 1.0f;
        }
    }
}