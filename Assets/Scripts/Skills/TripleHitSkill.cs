using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    private TimingEventResult result;

    public TripleHitSkill()
    {
        skillName = "Triple Hit";
        description = "Hits the enemy 3 times with timing checks before each hit";
        requiresMovement = true;
        energyCost = 3;
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
       
        
        // First Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack1Trigger");
        if (result == TimingEventResult.Miss)
            yield break;

            yield return  user.animationEnded == true;

        // Second Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target,"Attack2Trigger");
        if (result == TimingEventResult.Miss)
            yield break;

            yield return  user.animationEnded == true;

        // Third Timing Event
        yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target, "Attack3Trigger");
        user.isAttacking = false;
        yield return user.animationEnded == false; //Resets the animation flag
       
        
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
                target.TakeDamage(1);
                AudioManager.instance.PlaySlashSound();
                user.GainEnergy((energyCost / 2));
                break;
            case TimingEventResult.Good:
                Debug.Log("Good Hit!");
                user.animator.SetTrigger(trigger);
                target.TakeDamage(1);
                AudioManager.instance.PlaySlashSound();
                break;
            case TimingEventResult.Miss:
                Debug.Log("Missed!");
                user.animator.SetTrigger("AttackFailTrigger");
                user.isAttacking = false;
                break;
        }
    }
}