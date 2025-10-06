using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActivityProgressPanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI activityNameText;
    public TextMeshProUGUI stepsToNextText;
    public TextMeshProUGUI totalStepsText;
    public TextMeshProUGUI expGainedText;
    public Slider progressSlider;

    [Header("Linked Data")]
    public string currentActivity = "Logging";
    public int stepsRequiredForTick = 100;
    private int accumulatedSteps;
    private int totalSteps;
    private int expGained;

    private void OnEnable()
    {
        PlayerData.OnStepsAdded += HandleStepsAdded;
        UpdateUI();
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= HandleStepsAdded;
    }

    private void HandleStepsAdded(int steps)
    {
        // Only accumulate if player is doing an activity
        if (string.IsNullOrEmpty(currentActivity))
            return;

        accumulatedSteps += steps;
        totalSteps += steps;

        if (accumulatedSteps >= stepsRequiredForTick)
        {
            PerformProductionTick();
            accumulatedSteps -= stepsRequiredForTick;
        }

        UpdateUI();
    }

    private void PerformProductionTick()
    {
        // Example: give resources and exp based on current activity
        expGained += 5; // Example fixed exp per tick

        Debug.Log($"{currentActivity} tick completed! +5 EXP.");
        // TODO: integrate with Building/Resource systems here if desired
    }

    private void UpdateUI()
    {
        if (activityNameText != null)
            activityNameText.text = $"Activity: {currentActivity}";

        if (stepsToNextText != null)
            stepsToNextText.text = $"Next: {stepsRequiredForTick - accumulatedSteps} steps";

        if (totalStepsText != null)
            totalStepsText.text = $"Total: {totalSteps} steps";

        if (expGainedText != null)
            expGainedText.text = $"EXP: {expGained}";

        if (progressSlider != null)
        {
            progressSlider.maxValue = stepsRequiredForTick;
            progressSlider.value = accumulatedSteps;
        }
    }

    // You can call this when player changes activity (e.g. Logging -> Mining)
    public void SetActivity(string newActivity, int stepRequirement)
    {
        currentActivity = newActivity;
        stepsRequiredForTick = stepRequirement;
        accumulatedSteps = 0;
        expGained = 0;
        UpdateUI();
    }
}
