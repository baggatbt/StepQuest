using System.Collections;
using UnityEngine;

public class SlimeAttackSkill : Skill
{
    
    public SlimeAttackSkill()
    {
        skillName = "Slime Attack";
        description = "The slime attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;

    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;

        // First Timing Event
       
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "SlimeAttack1Trigger", result);
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
    

