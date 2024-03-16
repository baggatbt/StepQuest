using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Skill
{
    private int numberOfAttacks;
    public Slash()
    {
        skillName = "Slash";
        description = "A slash that deals " + PlayerData.Instance.attackPower + " damage.";
        energyCost = 0; 
        energyGain = 1;
        skillLevel = 1; 
        requiresMovement = true; 
        skillExecutionComplete = false;
        numberOfAttacks = 1;
        iconImage = LoadIconImage("SkillIcons/Icon4");
        
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

        user.animator.SetTrigger("Attack1Trigger");
        for( int i = 0; i < numberOfAttacks; i++)
        {
             

             yield return TimingWindow(user, target, battleManager, 0.0f, 0.5f);
             

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             AudioManager.instance.PlaySlashSound();
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
        }
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);
        //user.GainEnergy(energyGain);
       // target.CheckForDeath();
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