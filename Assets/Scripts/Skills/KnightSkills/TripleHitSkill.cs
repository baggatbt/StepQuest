using System.Collections;
using UnityEngine;
using System;

public class TripleHitSkill : Skill
{
    private int numberOfAttacksPossible;

    public TripleHitSkill()
    {
        // Identity
        Type = SkillType.TripleHit;
        skillName = "Triple Slash";
        description = "Tap before each hit for extra damage";

        // Energy / XP / misc
        energyCost = 3;
        energyGain = 0;
        energyGainBonus = 0;
        skillLevel = 1;
        skillPointCost = 1;
        requiresMovement = true;
        skillExecutionComplete = false;

        // Icon
        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_TripleSlash");

        // Behavior
        numberOfAttacksPossible = 3;

        // ✅ NEW ATK-BASED DAMAGE (replaces power = 78)
        damageMultiplier = 1.3f;          // Slightly stronger than Slash overall
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.2f;

        // Speed
        speedMultiplier = 1.00f;

        priority = 0;

        useVariance = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        // ✅ NEW: ATK-based total damage
        int totalDamageBudget = RollBaseDamage(user);

        // Triple hit split:
        // 30% / 30% / 40%
        int firstHit = Mathf.Max(1, Mathf.RoundToInt(totalDamageBudget * 0.30f));
        int secondHit = Mathf.Max(1, Mathf.RoundToInt(totalDamageBudget * 0.30f));
        int thirdHit = Mathf.Max(1, totalDamageBudget - firstHit - secondHit);

        int[] hits = { firstHit, secondHit, thirdHit };

        user.animator.SetTrigger("TripleSlashTrigger");

        for (int i = 0; i < hits.Length; i++)
        {
            int perHitDamage = hits[i];

            yield return TimingManager.Instance.HandleTimingWindow(
                user,
                target,
                perHitDamage,
                (TimingEventResult result) =>
                {
                    HandleTimingResultForPlayerAttack(user, target, result, perHitDamage);
                    battleManager.CameraShakeMagnitude(result);
                    battleManager.ShowTimingResult(result.ToString());
                });
        }

        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;
        bonusGained = false;

        target.CheckForDeath();
    }
}