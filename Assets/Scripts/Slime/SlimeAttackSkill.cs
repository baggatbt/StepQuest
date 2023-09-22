using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    private int numberOfAttacksPossible;
    public SlimeAttackSkill()
    {
        skillName = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;
        numberOfAttacksPossible = 1;

    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        int baseDamage = CalculateBaseDamage(user);
        user.animator.SetTrigger("SlimeAttack1Trigger");
        for( int i = 1; i < numberOfAttacksPossible; i++)
        {
             user.isAnimationDone = false;

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.6f);
             

             HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
             Debug.Log("Timing for player attack has been handled waiting for animations");
             yield return new WaitUntil(() => user.animationDamageTime == true);
             

             
        }
        
        user.isAttacking = false;
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
    

