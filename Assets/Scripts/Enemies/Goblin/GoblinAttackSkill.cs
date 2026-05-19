using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public GoblinAttackSkill()
    {
        Type = SkillType.None;

        skillName = "Goblin Attack";
        description = "The Goblin lunges forward with a quick strike. Time your block to reduce the damage.";

        energyCost = 0;
        energyGain = 0;
        energyGainBonus = 0;

        requiresMovement = true;
        skillExecutionComplete = false;

        numberOfAttacksPossible = 1;

        // Level 1 Goblin ATK is about 5.
        // 0.90x keeps the basic hit around 4-5 damage before defense/block.
        damageMultiplier = 0.90f;

        // Keep enemy damage predictable for now.
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.0f;
        useVariance = false;

        // Goblin is a little faster than Knight.
        speedMultiplier = 1.10f;

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
            user.animator.SetTrigger("GoblinAttack1Trigger");
        }
        else
        {
            Debug.LogWarning("[GoblinAttackSkill] User has no animator.");
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
            yield return new WaitForSeconds(0.35f);
        }

        user.isAnimationDone = false;
        user.isAttacking = false;
        user.damageApplied = false;

        target.CheckForDeath();
    }
}