using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    
    private int numberOfHits;
    public GoblinAttackSkill()
    {
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;
        numberOfHits = 2;

    }

     public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        user.animator.SetTrigger("GoblinAttack1Trigger");
        for (int i = 0; i < numberOfHits; i++)
        {

        
        // First Timing Event
        Debug.Log("first attack");
        yield return TimingWindow(user, target, battleManager, 0.0f, 0.7f);
        HandleTimingResult(user, target, "GoblinAttack1Trigger", result);

        
        

        yield return new WaitForSeconds(.5f);
        }
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