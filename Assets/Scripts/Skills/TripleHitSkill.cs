using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    private TimingEventResult result;

    public TripleHitSkill()
    {
        name = "Triple Hit";
        description = "Hits the enemy 3 times with timing checks before each hit";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        // First Timing Event
        yield return TimingWindow("Attack1Trigger",user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target);
        if (result == TimingEventResult.Miss)
            yield break;

            yield return new WaitForSeconds(1.0f); // delay before next timing event

        // Second Timing Event
        yield return TimingWindow("Attack2Trigger",user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target);
        if (result == TimingEventResult.Miss)
            yield break;

            yield return new WaitForSeconds(1.0f); // delay before next timing event

        // Third Timing Event
        yield return TimingWindow("Attack3Trigger",user, target, battleManager, 0.0f, 1.0f);
        HandleTimingResult(user, target);
        
    }


    private IEnumerator TimingWindow(string trigger,Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
        user.animator.SetTrigger(trigger);
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
    }


    private void HandleTimingResult(Character user, Character target)
    {
        switch (result)
        {
            case TimingEventResult.Perfect:
                Debug.Log("Perfect Hit!");
                target.TakeDamage(1);
                break;
            case TimingEventResult.Good:
                Debug.Log("Good Hit!");
                target.TakeDamage(1);
                break;
            case TimingEventResult.Miss:
                Debug.Log("Missed!");
                user.animator.SetTrigger("AttackFailTrigger");
                break;
        }
    }
}