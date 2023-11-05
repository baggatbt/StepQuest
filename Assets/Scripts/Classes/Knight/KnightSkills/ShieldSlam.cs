using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSlam : Skill
{
    private int numberOfAttacks;
    public ShieldSlam()
    {
        skillName = "Shield Slam";
        description = "A skill to lower enemy rage";
        energyCost = 0; 
        energyGain = 2;
        skillLevel = 1; 
        requiresMovement = true; 
        skillExecutionComplete = false;
        numberOfAttacks = 1;
        int energyToTakeFromEnemy = -1; //Must be negative in order to use GainEnergy() from Character
        
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

        for( int i = 0; i < numberOfAttacks; i++)
        {
            
             
             yield return TimingWindow(user, target, battleManager, 0.0f, 0.9f);
             

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
        }
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);

        if (result != TimingEventResult.Miss)
            {
                //Reduce the targets energy by 1 *didnt use spend energy because thats for the players temp energy only
                target.GainEnergy(energyToTakeFromEnemy);
            
            }
        user.animationDamageTime = false;
        user.isAnimationDone = false;
        user.isAttacking = false;
        target.CheckForDeath();
    }


    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerHoldReleaseTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            user.animator.SetTrigger("ShieldSlamTrigger");
            result = timingResult;
            
        }));
    }


}