using System.Collections;
using UnityEngine;

public class MushroomAttackSkill : Skill
{
    private int numberOfAttacksPossible;

    public MushroomAttackSkill()
    {
        // Identity
        skillName = "Mushroom Attack";
        description = "A heavy swing. Slower, but hits harder.";
        requiresMovement = true;
        numberOfAttacksPossible = 1;

        // NEW: power + speed
        // Beefier than the goblin's 60-power poke
        power = 75;

        // Slower-feeling action speed
        speedMultiplier = 0.90f;

        // Optional: if you ever want heavy moves to "lose ties"
        priority = 0;

        // Keep variance off while prototyping timing + balance
        useVariance = false;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;

        // NEW: power-based incoming damage (defense applied in target.TakeDamage)
        int baseDamage = CalculateBaseDamage(
            userAttack: user.attackPower,
            targetDefense: target.defensePower, // ignored by your current helper (fine)
            userLevel: user.level
        );

        user.animator.SetTrigger("MushroomAttack1Trigger");

        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            yield return TimingManager.Instance.HandleTimingWindow(
                user, target, baseDamage,
                (TimingEventResult result) =>
                {
                    HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
                    battleManager.CameraShakeMagnitude(result);
                }
            );
        }

        yield return new WaitUntil(() => user.isAnimationDone);

        user.isAnimationDone = false;
        user.isAttacking = false;

        target.CheckForDeath();
    }
}