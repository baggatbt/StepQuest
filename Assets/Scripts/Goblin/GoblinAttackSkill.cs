using System.Collections;
using UnityEngine;

public class GoblinAttackSkill : Skill
{
    private TimingEventResult result;
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

        for (int i = 0; i < numberOfHits; i++)
        {

        
        // First Timing Event
        Debug.Log("first attack");
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "GoblinAttack1Trigger");

        
        

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


    private void HandleTimingResult(Character user, Character target, string trigger)
    {
        switch (result)
        {
            case TimingEventResult.Perfect:
                Debug.Log("Perfect Block!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(user.damage - user.damage);
                target.animator.SetTrigger("BlockTrigger");
                AudioManager.instance.PlaySlashSound();
                break;
            case TimingEventResult.Good:
                Debug.Log("Good Block!");   
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