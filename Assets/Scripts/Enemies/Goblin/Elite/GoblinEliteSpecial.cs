using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoblinEliteSpecial : Skill
{
    private int numberOfAttacksPossible;

    public GoblinEliteSpecial()
    {
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        numberOfAttacksPossible = 4;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 0.8f);  // 80% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;  

        int baseDamage = CalculateBaseDamage(user);

        user.animator.SetTrigger("GoblinSpecialAttackTrigger");

        for (int i = 0; i < numberOfAttacksPossible; i++)
        {
            
            yield return TimingWindow(user, target, battleManager, 0.0f, 0.8f);

            HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
            Debug.Log("Timing for player attack has been handled waiting for animations");

            yield return new WaitUntil(() => user.animationDamageTime == true);
            Debug.Log(user.isAnimationDone + "right after the damagetime");
        }

        Debug.Log("waiting on animation to finish");
        
        yield return new WaitUntil(() => user.isAnimationDone == true);
        user.animationDamageTime = false;
        user.isAnimationDone = false;
        user.isAttacking = false;
        user.energy = 0;
        target.CheckForDeath();
    }

    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
    }
}
