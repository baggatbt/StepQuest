using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public static TimingManager Instance; // Singleton instance

    private void Awake()
    {
        // Ensure only one instance of TimingManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to handle timing and damage application
    public IEnumerator HandleTimingWindow(Character user, Character target, int baseDamage, System.Action<TimingEventResult> onDamageApplied)
    {
        // Ensure the flag is reset at the start of the timing window
        user.damageApplied = false;

        // Wait for the timing window to open
        yield return new WaitUntil(() => user.isWindowOpen);

        // Check for player input during the window
        bool successfulTiming = false;
        bool inputAttempted = false;

        while (user.isWindowOpen)
        {
            if (!inputAttempted && user.CheckPlayerInput())
            {
                successfulTiming = true;
                inputAttempted = true; // Mark that the player has made an attempt
                break;
            }
            yield return null; // Wait until the next frame
        }

        // Apply damage only if it hasn't been applied yet
        if (!user.damageApplied)
        {
            // Determine the result
            TimingEventResult result = successfulTiming ? TimingEventResult.Good : TimingEventResult.Miss;

            // Call the provided action to handle damage
            onDamageApplied(result);

            // Mark that damage has been applied
            user.damageApplied = true;
        }

        // Ensure the flag is not reset too early
        yield return new WaitUntil(() => !user.isWindowOpen);
    }
}
