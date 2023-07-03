using System.Collections;
using UnityEngine;

public class TripleHitSkill : Skill
{
    public TripleHitSkill()
    {
        name = "Triple Hit";
        description = "Hits the enemy 3 times with timing checks before each hit";
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
    // Reset the skillExecutionComplete flag at the start
    skillExecutionComplete = false;

    user.animator.SetTrigger("TripleHitTrigger");
    yield return TimingWindow(user, target, battleManager, 0.2f, 0.5f);
    if (!timingSuccess)
        yield break; // Stop skill execution if timing event failed

    user.animator.SetTrigger("TripleHitTrigger2");
    yield return TimingWindow(user, target, battleManager, 1.5f, 1.8f);
    if (!timingSuccess)
        yield break; // Stop skill execution if timing event failed

    user.animator.SetTrigger("TripleHitTrigger3");
    yield return TimingWindow(user, target, battleManager, 2.8f, 3.3f);
    if (!timingSuccess)
        yield break; // Stop skill execution if timing event failed

    // Set the skillExecutionComplete flag to true at the end
    skillExecutionComplete = true;
}


private bool timingSuccess = false;

private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
{
    timingSuccess = false;
    yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (result) =>
    {
        timingSuccess = result;
        if (result)
        {
            Debug.Log("Successful Hit!");
            target.TakeDamage(1);
        }
        else
        {
            Debug.Log("Attempting to cancel animation!");
            user.animator.SetTrigger("CancelAnimation"); //trigger to cancel anim in case of time event failure
        }
    }));
}

}