using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    private TimingEventResult result;
    public GoblinAttackSkill()
    {
        skillName = "Goblin Attack";
        description = "The Goblin attacks the player. The damage can be reduced by timely action.";
        requiresMovement = true;
        energyCost = 0;

    }

     public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
       
        
        // First Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "GoblinAttack1Trigger");
        

        // Second Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target,"GoblinAttack2Trigger");
       

    }


    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
         
    }


    private void HandleTimingResult(Character user, Character target, string trigger)
    {
        switch (result)
        {
            case TimingEventResult.Perfect:
                Debug.Log("Perfect Hit!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(user.damage - user.damage);
                AudioManager.instance.PlaySlashSound();
                break;
            case TimingEventResult.Good:
                Debug.Log("Good Hit!");   
                user.animator.SetTrigger(trigger);            
                target.TakeDamage(user.damage - 1);
                AudioManager.instance.PlaySlashSound();
                break;
            case TimingEventResult.Miss:
                user.animator.SetTrigger(trigger);
                target.TakeDamage(user.damage);            
                
                break;
        }
    }
    
}