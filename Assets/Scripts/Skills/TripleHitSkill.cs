using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    

    

    public TripleHitSkill()
    {
        skillName = "Triple Slash";
        description = "Slash up to three times with good timing";
        requiresMovement = true;
        energyCost = 0;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
       
        
        // First Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack1Trigger", result);
        if (result == TimingEventResult.Miss)
            yield break;
            yield return  user.animationEnded == true;


        if (target.health >= 1)
        {
        // Second Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack2Trigger", result);
        if (result == TimingEventResult.Miss)
            yield break;
            yield return  user.animationEnded == true;
        }
        
        if (target.health >= 1)
        {
        // Third Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack3Trigger", result);
        yield return new WaitUntil(() => user.animationEnded == true); //Resets the animation flag
        user.isAttacking = false;
        }
        
        else 
        {
            user.isAttacking = false;
            user.animator.SetTrigger("AttackFailTrigger");
            yield return user.animationEnded == true; //Resets the animation flag
        }
    }


    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
         
    }


}