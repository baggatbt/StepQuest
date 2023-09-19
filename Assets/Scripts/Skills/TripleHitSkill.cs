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

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 0.5f);  // 70% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        int baseDamage = CalculateBaseDamage(user);
        
        
        
        // First Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResultForPlayerAttack(user, target, "Attack1Trigger", result, baseDamage);
        if (result == TimingEventResult.Miss)
            {
                user.animator.SetTrigger("Attack1Trigger");
                yield break;
            }
        yield return new WaitUntil(() => user.animationEnded);



        if (target.health >= 1)
        {
        // Second Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResultForPlayerAttack(user, target, "Attack2Trigger", result, baseDamage);
        if (result == TimingEventResult.Miss)
            yield break;
            yield return  user.animationEnded == true;
        }
        
        if (target.health >= 1)
        {
        // Third Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResultForPlayerAttack(user, target, "Attack3Trigger", result, baseDamage);
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