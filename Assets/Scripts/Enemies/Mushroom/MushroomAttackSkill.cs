using System.Collections;
using UnityEngine;

public class MushroomAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public MushroomAttackSkill()
    {
        Type = SkillType.None;

        skillName = "Mushroom Slam";
        description = "A slow, heavy strike. Time your block to reduce the damage.";

        energyCost = 0;
        energyGain = 0;
        energyGainBonus = 0;

        requiresMovement = true;
        skillExecutionComplete = false;

        numberOfAttacksPossible = 1;

        // Level 1 Mushroom ATK is about 6.
        // 1.10x makes this hit for around 6-7 before defense/block.
        damageMultiplier = 1.10f;

        // Keep enemy damage predictable while prototyping.
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.0f;
        useVariance = false;

        // Heavy enemy, slower action.
        speedMultiplier = 0.85f;

        priority = 0;

        iconImage = null;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;
        user.damageApplied = false;

        int baseDamage = RollBaseDamage(user);
        baseDamage = Mathf.Max(1, baseDamage);

        if (user.animator != null)
        {
            user.animator.SetTrigger("MushroomAttack1Trigger");
        }
        else
        {
            Debug.LogWarning("[MushroomAttackSkill] User has no animator.");
        }

        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            yield return TimingManager.Instance.HandleTimingWindow(
                user,
                target,
                baseDamage,
                (TimingEventResult result) =>
                {
                    HandleTimingResultForEnemyAttack(user, target, result, baseDamage);

                    if (battleManager != null)
                        battleManager.CameraShakeMagnitude(result);
                });
        }

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

        target.CheckForDeath();
    }
}