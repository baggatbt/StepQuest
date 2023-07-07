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
    yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f); //How long the window is for a success timing
    if (!timingSuccess)
        yield break; // Stop skill execution if timing event failed

    user.animator.SetTrigger("TripleHitTrigger2");
    yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
    if (!timingSuccess)
        yield break; // Stop skill execution if timing event failed

    user.animator.SetTrigger("TripleHitTrigger3");
    yield return TimingWindow(user, target, battleManager, 0.0f, 1.0f);
    if (!timingSuccess)
        yield break; // Stop skill execution if timing event failed

    // Set the skillExecutionComplete flag to true at the end
    skillExecutionComplete = true;
}


private bool timingSuccess = false;

    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
        timingSuccess = false;
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd,
            (result) =>
            {
                if (result == TimingEventResult.Success)
                {
                    Debug.Log("Successful Hit!");
                    target.TakeDamage(1);
                    timingSuccess = true;
                }
                else if (result == TimingEventResult.RightClickSuccess)
                {
                    Debug.Log("Right click success!");
                // Handle the right click success case (for now it will have none, only useful for enemy attacks)
            }
                else
                {
                    Debug.Log("Attempting to cancel animation!");
                    user.animator.SetTrigger("CancelAnimation");
                }
            }));
    }


}