using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public GoblinAttackSkill()
    {
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";

        energyCost = 0;
        energyGain = 0;
        energyGainBonus = 0;

        requiresMovement = true;
        skillExecutionComplete = false;

        numberOfAttacksPossible = 1;

        // NEW ATK-based damage system
        damageMultiplier = 1.0f;      // 100% of Goblin ATK
        damageVarianceMin = 1.0f;
        damageVarianceMax = 1.2f;

        // Goblin basic attack is fairly quick
        speedMultiplier = 1.05f;

        priority = 0;
        useVariance = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        // NEW: ATK-based damage instead of old power formula
        int baseDamage = RollBaseDamage(user);

        user.animator.SetTrigger("GoblinAttack1Trigger");

        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            yield return TimingManager.Instance.HandleTimingWindow(
                user,
                target,
                baseDamage,
                (TimingEventResult result) =>
                {
                    HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
                    battleManager.CameraShakeMagnitude(result);
                });
        }

        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;

        target.CheckForDeath();
    }
}