using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    private int numberOfAttacksPossible;

    public TripleHitSkill()
    {
        skillName = "Triple Slash";
        description = "Slash three times";
        requiresMovement = true;
        energyCost = 0;
        numberOfAttacksPossible = 4;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 0.6f);  // 60% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;  // Reset the flag at the start of each attack



        int baseDamage = CalculateBaseDamage(user);
        user.animator.SetTrigger("TripleSlashTrigger");
        for( int i = 1; i < numberOfAttacksPossible; i++)
        {
             

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.6f);
             

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
        }
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);
        user.animationDamageTime = false;
        user.isAnimationDone = false;
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