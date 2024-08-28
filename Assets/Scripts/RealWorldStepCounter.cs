using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RealWorldStepCounter : MonoBehaviour
{
    private StepCounter stepCounter;
    private int simulatedSteps = 0;

    void Start()
    {
        stepCounter = StepCounter.current;

        if (stepCounter == null)
        {
            Debug.LogWarning("No StepCounter device found. Using simulated steps.");
        }
        else
        {
            Debug.Log("StepCounter found.");
        }
    }

    void Update()
    {
        // Simulate steps by pressing the "S" key
        if (Input.GetKeyDown(KeyCode.S))
        {
            SimulateStep();
        }

        // In case of a real step counter, handle real steps
        if (stepCounter != null)
        {
            int steps = stepCounter.stepCounter.ReadValue();
            
            //Debug.Log("Steps taken: " + steps);
           // Debug.Log("Current Steps: " + PlayerManager.Instance.playerAccountData.currentSteps);

            if (steps > PlayerManager.Instance.playerAccountData.currentSteps)
            {
                int stepsTaken = steps - PlayerManager.Instance.playerAccountData.currentSteps;
                for (int i = 0; i < stepsTaken; i++)
                {
                    PlayerManager.Instance.IncrementInGameSteps();
                }

                PlayerManager.Instance.playerAccountData.currentSteps = steps;
                PlayerManager.Instance.SavePlayerAccountData();
             
            }
        }
        if (stepCounter == null)
        {
          //  Debug.LogError("StepCounter device not found.");
        }
    }

    private void SimulateStep()
    {
        simulatedSteps++;
        PlayerManager.Instance.IncrementInGameSteps();
        Debug.Log("Simulated step taken. Total simulated steps: " + simulatedSteps);
    }
}
