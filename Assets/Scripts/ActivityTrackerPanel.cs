using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
Attach this to a UI panel to show live mission progress for one activity (e.g., LoggingOne / Woodcutting):
 • Activity name + skill id
 • Steps to next reward (accounts for efficiency per level)
 • Effective SPR
 • Pending rewards / capacity
 • Unclaimed XP
 • Total steps contributed (lifetime, optional)
 • Two sliders:
    - leftoverSteps / effective SPR   (per-tick progress)
    - pending / capacity              (storage fill)
*/
public class ActivityTrackerPanel : MonoBehaviour
{
    [Header("Mission to Track")]
    public string missionID = "LoggingOne";          // set in Inspector
    public string displayNameOverride = "Logging";   // optional friendly label

    [Header("UI")]
    public TextMeshProUGUI activityNameText;   // "Logging (Woodcutting)"
    public TextMeshProUGUI stepsToNextText;    // "Next in: X steps"
    public TextMeshProUGUI sprText;            // "SPR: X"
    public TextMeshProUGUI pendingText;        // "Stored: P / C"
    public TextMeshProUGUI xpPendingText;      // "Unclaimed XP: N"
    public TextMeshProUGUI totalStepsText;     // "Total Steps: N"
    public Slider          stepProgressSlider; // leftover / SPR
    public Slider          rewardCapacitySlider; // pending / capacity

    private void OnEnable()
    {
        PlayerData.OnStepsAdded += HandleStepsAdded;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= HandleStepsAdded;
    }

    private void HandleStepsAdded(int _) => Refresh();

    private void Update() // optional: keeps panel fresh even without new steps
    {
        Refresh();
    }

    private void Refresh()
    {
        var mgr = MissionManager.Instance;
        if (mgr == null) return;

        string skillID   = mgr.GetSkillID(missionID);
        string niceName  = string.IsNullOrEmpty(displayNameOverride) ? missionID : displayNameOverride;

        int spr        = mgr.GetEffectiveStepsPerReward(missionID);
        int toNext     = mgr.GetStepsToNextReward(missionID);
        int leftover   = mgr.GetLeftoverSteps(missionID);
        int pending    = mgr.GetPendingRewardCount(missionID);
        int capacity   = mgr.GetRewardCapacity(missionID);
        int xpPending  = mgr.GetPendingUnclaimedXP(missionID);
        int totalSteps = mgr.GetTotalStepsAccumulated(missionID);

        if (activityNameText) activityNameText.text = $"{niceName} ({skillID})";
        if (stepsToNextText)  stepsToNextText.text  = $"Next in: {toNext} steps";
        if (sprText)          sprText.text          = $"SPR: {spr}";
        if (pendingText)      pendingText.text      = $"Stored: {pending} / {capacity}";
        if (xpPendingText)    xpPendingText.text    = $"Unclaimed XP: {xpPending}";
        if (totalStepsText)   totalStepsText.text   = $"Total Steps: {totalSteps}";

        if (stepProgressSlider)
        {
            stepProgressSlider.maxValue = Mathf.Max(1, spr);
            stepProgressSlider.value    = Mathf.Clamp(leftover, 0, stepProgressSlider.maxValue);
        }

        if (rewardCapacitySlider)
        {
            rewardCapacitySlider.maxValue = Mathf.Max(1, capacity);
            rewardCapacitySlider.value    = Mathf.Clamp(pending, 0, rewardCapacitySlider.maxValue);
        }
    }
}
