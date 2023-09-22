using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    
    private int numberOfAttacksPossible;
    public GoblinAttackSkill()
    {
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;
        numberOfAttacksPossible = 4;

    }

     // Override the default base damage calculation.
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
        user.animator.SetTrigger("GoblinAttack1Trigger");
        for( int i = 1; i < numberOfAttacksPossible; i++)
        {
             

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.6f);
             

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             Debug.Log(user.isAnimationDone + "right before the damagetime");
             yield return new WaitUntil(() => user.animationDamageTime == true);
             Debug.Log(user.isAnimationDone + "right after the damagetime");
             

             
        }
        Debug.Log("waiting on animation to finish");
        user.isAnimationDone = true;
        Debug.Log(user.isAnimationDone + "right before the yield");
        yield return new WaitUntil(() => user.isAnimationDone == true);
        Debug.Log(user.isAnimationDone + "right after the yield");
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


