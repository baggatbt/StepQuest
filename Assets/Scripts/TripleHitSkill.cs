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
        int hitCount = 2;
        int successfulHits = 0;  // Number of successful hits by the player
        

        // Set the desired timing windows for each hit
        float[] windowStarts = { 0.0f, 0.0f };
        float[] windowEnds = { 5.0f, 5.0f };

        // The damage of the first hit
        target.TakeDamage(1);

        // Pass the arrays of window starts and ends to PlayerActiveTimeEvent
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStarts, windowEnds, (result) =>
        {
            if (result)
            {
                successfulHits++;
                Debug.Log(successfulHits);
                target.TakeDamage(1);
            }
        }));

        if (successfulHits == hitCount)
        {
            // Player successfully hit all the times
            // Perform any necessary actions here
            user.currentSkill.skillExecutionComplete = true; // Set the flag on the user
            battleManager.EnemyAttack();
            Debug.Log("Skill execution complete");
        }
        else
        {
            // Player missed at least once
            user.currentSkill.skillExecutionComplete = true; // Set the flag on the user
            battleManager.EnemyAttack();
            Debug.Log("Skill execution interrupted");
        }

        skillExecutionComplete = true; // Set the flag to indicate skill execution is complete

       
    }
}
