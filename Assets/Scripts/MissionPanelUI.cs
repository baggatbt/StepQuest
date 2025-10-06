using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
UI: Start/Running/Claim panel.
- Shows stored "pending / capacity" and a normalized capacity bar.
- Lets the player Start a mission or Claim rewards once available.
*/
public class MissionPanelUI : MonoBehaviour
{
    [Header("Inspector links")]
    public string           missionID;
    public Slider           progressBar;       // pending / capacity
    public Button           actionButton;      // Start / Claim
    public TextMeshProUGUI  actionText;
    public TextMeshProUGUI  rewardCountText;   // “pending / capacity”

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
            bool slotFree = mgr.ActiveMissionCount < 2; // match your maxActive
            actionButton.interactable = slotFree;
        }
    }

    private void OnActionPressed()
    {
        var mgr = MissionManager.Instance;
        if (mgr == null) return;

        if (mgr.HasRewards(missionID))
            mgr.ClaimMission(missionID);
        else
            mgr.StartMission(missionID);
    }
}