using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Start/Running/Claim panel with confirmation on swap.
/// Requires MissionManager (with StartMission / StartOrSwapMission).
/// Optional: assign swapNoteText to show which mission was stopped.
/// </summary>
public class MissionPanelUI : MonoBehaviour
{
    [Header("Inspector links")]
    public string           missionID;
    public Slider           progressBar;       // pending / capacity
    public Button           actionButton;      // Start / Claim / Running
    public TextMeshProUGUI  actionText;
    public TextMeshProUGUI  rewardCountText;   // “pending / capacity”
    public TextMeshProUGUI  swapNoteText;      // optional

    private void Awake()
    {
        if (actionButton != null)
            actionButton.onClick.AddListener(OnActionPressed);
    }

    private void Update()
    {
        var mgr = MissionManager.Instance;
        if (mgr == null) return;

        int pending  = mgr.GetPendingRewardCount(missionID);
        int capacity = mgr.GetRewardCapacity(missionID);

        if (rewardCountText) rewardCountText.text = $"{pending} / {capacity}";
        if (progressBar)
        {
            progressBar.maxValue = Mathf.Max(1, capacity);
            progressBar.value    = Mathf.Clamp(pending, 0, progressBar.maxValue);
        }

        if (mgr.HasRewards(missionID))
        {
            if (actionText) actionText.text = "Claim";
            actionButton.interactable = true;
        }
        else if (mgr.IsActive(missionID))
        {
            if (actionText) actionText.text = "Running";
            actionButton.interactable = false;
        }
        else
        {
            if (actionText) actionText.text = "Start";
            actionButton.interactable = true; // we’ll confirm swap if needed
        }
    }

    private void OnActionPressed()
    {
        var mgr = MissionManager.Instance;
        if (mgr == null) return;

        // 1) Claim if rewards exist
        if (mgr.HasRewards(missionID))
        {
            mgr.ClaimMission(missionID);
            if (swapNoteText) swapNoteText.text = "";
            return;
        }

        // 2) If mission already running, nothing to do
        if (mgr.IsActive(missionID))
            return;

        // 3) Try to start normally (if capacity available)
        if (mgr.ActiveMissionCount < GetMaxActiveMissions())
        {
            mgr.StartMission(missionID);
            if (swapNoteText) swapNoteText.text = "";
            return;
        }

        // 4) Capacity full → ask for confirmation and swap
        string activeId = mgr.GetFirstActiveMissionId(excludeId: missionID);
        if (string.IsNullOrEmpty(activeId))
        {
            // No active to swap, bail safely
            return;
        }

        // Show confirmation dialog
        if (ConfirmDialog.Instance != null)
        {
            string title = "Swap Activity?";
            string msg   = $"Stop '{activeId}' and start '{missionID}'?\n" +
                           $"Stored rewards on '{activeId}' are kept.";
            ConfirmDialog.Instance.Show(
                title, msg, "Swap", "Cancel",
                onConfirm: () =>
                {
                    string stopped = mgr.StartOrSwapMission(missionID);
                    if (!string.IsNullOrEmpty(stopped) && swapNoteText)
                        swapNoteText.text = $"Swapped from: {stopped}";
                });
        }
        else
        {
            // Fallback: swap immediately if no dialog present
            string stopped = mgr.StartOrSwapMission(missionID);
            if (!string.IsNullOrEmpty(stopped) && swapNoteText)
                swapNoteText.text = $"Swapped from: {stopped}";
        }
    }

    // Reads the serialized cap from MissionManager (default 1 if not present)
    private int GetMaxActiveMissions()
    {
        // If you exposed it as a serialized field on MissionManager:
        // Make a getter there. For now, replicate intent:
        // We consider capacity "full" if ActiveMissionCount >= 1 (swap mode).
        return 1;
    }
}
