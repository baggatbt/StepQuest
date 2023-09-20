using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StepGoalTracker : MonoBehaviour
{

    public Slider goalTrackerBar;

    private int currentStepsTowardDailyGoal;
    private StepCounterController stepCounterController;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentStepsTowardDailyGoal = PlayerData.Instance.inGameSteps;
        goalTrackerBar.value = currentStepsTowardDailyGoal;
    }

    private void GetSteps()
    {

    }

    private void ResetSteps()
    {
        currentStepsTowardDailyGoal = 0;
    }

}
