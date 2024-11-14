using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimingManager : MonoBehaviour
{
    public static TimingManager Instance; // Singleton instance
    
    public GameObject timingWindowVisualAid; // Reference to the timing window visual aid GameObject

    private void Awake()
    {
        // Ensure only one instance of TimingManager exists
        if (Instance == null)
        {
            Instance = this;


            // Find timingWindowVisualAid GameObject by name if it hasn’t been assigned in the Inspector
            if (timingWindowVisualAid == null)
            {
                timingWindowVisualAid = GameObject.Find("timingWindowVisualAid");
                if (timingWindowVisualAid == null)
                {
                    Debug.LogWarning("timingWindowVisualAid GameObject not found in the scene.");
                }
            }
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

        // Show timing aid if it exists
        ShowTimingAid();

        bool successfulTiming = false;

        while (user.isWindowOpen)
        {
            if (!inputAttempted && user.CheckPlayerInput())
            {
                successfulTiming = true;
                inputAttempted = true;
                break;
            }
            yield return null;
        }

        // Hide timing aid once the window closes
        HideTimingAid();

        // Determine the result based on whether input was attempted successfully
        TimingEventResult result = successfulTiming ? TimingEventResult.Good : TimingEventResult.Miss;

        // Call the provided action to handle damage
        onDamageApplied(result);

        // Ensure the timing window is fully closed before exiting
        yield return new WaitUntil(() => !user.isWindowOpen);
    }

    public void ShowTimingAid()
    {
        if (timingWindowVisualAid != null)
        {
            timingWindowVisualAid.SetActive(true);
        }
    }

    public void HideTimingAid()
    {
        if (timingWindowVisualAid != null)
        {
            timingWindowVisualAid.SetActive(false);
        }
    }
}
