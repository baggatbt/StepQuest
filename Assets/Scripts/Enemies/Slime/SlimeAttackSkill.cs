using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    private int numberOfAttacks;
    public SlimeAttackSkill()
    {
        skillName = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;
        numberOfAttacks = 1;

    }

     protected override int CalculateBaseDamage(Character user)
    {
        return user.attackPower; // 100% of user's attack power
    }

     public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.isAnimationDone = false;  // Reset the flag at the start of each attack



        int baseDamage = CalculateBaseDamage(user);

        user.animator.SetTrigger("SlimeAttack1Trigger");
        for( int i = 0; i < numberOfAttacks; i++)
        {
             

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.7f);
             

             HandleTimingResultForEnemyAttack(user, target, result, baseDamage);
             Debug.Log("Timing for enemy attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
        }
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);
        user.animationDamageTime = false;
        user.isAnimationDone = false;
        user.isAttacking = false;
        
    }


    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
         
    }


}
    

