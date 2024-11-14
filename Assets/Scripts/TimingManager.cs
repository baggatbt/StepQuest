using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimingManager : MonoBehaviour
{
    public static TimingManager Instance; // Singleton instance
    private TimingVisualAid visualAid;

    private void Awake()
    {
        // Ensure only one instance of TimingManager exists
        if (Instance == null)
        {
            Instance = this;
            visualAid = FindObjectOfType<TimingVisualAid>(); // Find and reference the visual aid
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to handle timing and damage application per window
    public IEnumerator HandleTimingWindow(Character user, Character target, int baseDamage, System.Action<TimingEventResult> onDamageApplied)
    {
        bool inputAttempted = false; // Local flag to track if the player has already attempted input in this window

        // Wait for the timing window to open
        yield return new WaitUntil(() => user.isWindowOpen);

        
        

        // Check for player input during the window
        bool successfulTiming = false;

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

       

        // Determine the result based on whether input was attempted successfully
        TimingEventResult result = successfulTiming ? TimingEventResult.Good : TimingEventResult.Miss;

        // Call the provided action to handle damage
        onDamageApplied(result);

        // Ensure the timing window is fully closed before exiting
        yield return new WaitUntil(() => !user.isWindowOpen);
    }
}
