using System.Collections;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfAttacksPossible;

    public Slash()
    {
        Type = SkillType.Slash;
        skillName = "Slash";
        description = "Tap at the right time for extra damage";

        energyCost = 0;
        energyGain = 1;
        energyGainBonus = 1;
        skillLevel = 1;
        requiresMovement = true;
        skillExecutionComplete = false;

        iconImage = LoadIconImage("Sprites/SkillIcons/Knight/Knight_Icon_Slash");

        numberOfAttacksPossible = 2;

        damageMultiplier = 1.0f;
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.2f;

        speedMultiplier = 1.10f;
        priority = 0;

        useVariance = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        int totalDamageBudget = RollBaseDamage(user);

        int firstHit = Mathf.CeilToInt(totalDamageBudget * 0.60f);
        int secondHit = Mathf.Max(1, totalDamageBudget - firstHit);

        int[] hits = { firstHit, secondHit };

        user.animator.SetTrigger("Attack1Trigger");

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

        user.GainEnergy(energyGain);
        target.CheckForDeath();
    }
}