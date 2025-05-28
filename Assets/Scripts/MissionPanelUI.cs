using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Step accumulation
• Each time OnStepsAdded(n) fires, we add those steps into leftoverSteps.
• Whenever leftoverSteps ≥ stepsPerReward, we convert one “block” of stepsPerReward into a single pending reward (up to your rewardCapacity) and subtract that many steps from leftoverSteps.

Drop‐table rolls
• Those pending rewards aren’t immediately granted—you just see them accumulate as a count (pendingRewards).
• When you hit Claim, we loop over each pending reward, roll your dropTable once per reward, and hand you the resulting items.

UI control
• Your slider shows how full pendingRewards / rewardCapacity is.
• Your text shows “X / Y” pending rewards.
• The Claim button only becomes interactable once you’ve generated at least one pending reward.

Partial steps carry‐over
• Any leftover steps that didn’t fill a whole block remain in leftoverSteps, so you never “lose” partial progress toward the next reward.
*/
public class MissionPanelUI : MonoBehaviour
{
    [Header("Inspector links")]
    public string           missionID;
    public Slider           progressBar;
    public Button           actionButton;
    public TextMeshProUGUI  actionText;
    public TextMeshProUGUI  rewardCountText;    // shows “pending / capacity”

    private void Awake()
    {
        actionButton.onClick.AddListener(OnActionPressed);
    }

    private void Update()
    {
        var mgr = MissionManager.Instance;

        // update stored‐rewards text
        int pending  = mgr.GetPendingRewardCount(missionID);
        int capacity = mgr.GetRewardCapacity(missionID);
        rewardCountText.text = $"{pending} / {capacity}";

        // update progress bar fill
        progressBar.value = mgr.GetProgress01(missionID);

        // choose button state
        if (mgr.HasRewards(missionID))
        {
            actionText.text        = "Claim";
            actionButton.interactable = true;
        }
        else if (mgr.IsActive(missionID))
        {
            actionText.text        = "Running";
            actionButton.interactable = false;
        }
        else
        {
            actionText.text        = "Start";
            bool slotFree          = mgr.ActiveMissionCount < 2; // match your maxActive
            actionButton.interactable = slotFree;
        }
    }

    private void OnActionPressed()
    {
        var mgr = MissionManager.Instance;

        if (mgr.HasRewards(missionID))
            mgr.ClaimMission(missionID);
        else
            mgr.StartMission(missionID);
    }
}
