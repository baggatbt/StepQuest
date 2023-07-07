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
        yield return TimingWindow(user, target, battleManager, 0.0f, 0.5f, 0.0f, 0.0f); // How long the window is for a success timing
        if (!timingSuccess)
            yield break; // Stop skill execution if timing event failed

        user.animator.SetTrigger("TripleHitTrigger2");
        yield return TimingWindow(user, target, battleManager, 0.0f, 0.5f, 0.0f, 0.0f);
        if (!timingSuccess)
            yield break; // Stop skill execution if timing event failed

        user.animator.SetTrigger("TripleHitTrigger3");
        yield return TimingWindow(user, target, battleManager, 0.0f, 0.5f, 0.0f, 0.0f);
        if (!timingSuccess)
            yield break; // Stop skill execution if timing event failed

        // Set the skillExecutionComplete flag to true at the end
        skillExecutionComplete = true;
    }

    private bool timingSuccess = false;

<<<<<<< Updated upstream
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
=======
    private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float leftClickStart, float leftClickEnd, float rightClickStart, float rightClickEnd)
    {
        timingSuccess = false;
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(leftClickStart, leftClickEnd, rightClickStart, rightClickEnd,
            (result) =>
            {
                if (result == TimingEventResult.LeftClickSuccess || result == TimingEventResult.RightClickSuccess)
                {
                    Debug.Log("Successful Hit!");
                    target.TakeDamage(1);
                    timingSuccess = true;
                }
                else
                {
                    Debug.Log("Attempting to cancel animation!");
                    user.animator.SetTrigger("CancelAnimation");
                }
            }));
    }
}
>>>>>>> Stashed changes
