using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public GoblinAttackSkill()
    {
        // Identity (optional but nice)
        // If you added this enum value, set it; otherwise you can remove this line.
        // Type = SkillType.GoblinAttack; 
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";

        requiresMovement = true;
        numberOfAttacksPossible = 1;

        // NEW: power + speed
        // For a basic enemy poke, keep it near your "basic" power.
        power = 60;

        // Goblin basic attack is fairly quick but not crazy.
        speedMultiplier = 1.05f;

        // Optional
        priority = 0;

        // Optional variance (usually off since you already have timing)
        useVariance = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        // NEW: power-based damage
        int baseDamage = CalculateBaseDamage(
            userAttack: user.attackPower,
            targetDefense: target.defensePower,
            userLevel: user.level
        );

        // Goblin attack animation
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