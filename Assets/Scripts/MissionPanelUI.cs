using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionPanelUI : MonoBehaviour
{
    [Header("Inspector links")]
    public string           missionID;
    public Slider           progressBar;
    public Button           actionButton;
    public TextMeshProUGUI  actionText;

    private void Awake()
    {
        actionButton.onClick.AddListener(OnActionPressed);
    }

    private void Update()
    {
        var mgr = MissionManager.Instance;
        progressBar.value = mgr.GetProgress01(missionID);

        if (mgr.IsComplete(missionID))
        {
            if (mgr.IsClaimed(missionID))   // never true now (flag resets)
            {
                actionText.text = "Done";
                actionButton.interactable = false;
            }
            else
            {
                actionText.text = "Claim";
                actionButton.interactable = true;
            }
        }
        else if (mgr.IsActive(missionID))
        {
            actionText.text = "Running";
            actionButton.interactable = false;
        }
        else                                 // neither complete nor active
{
    actionText.text = "Start";

    bool slotFree = MissionManager.Instance.ActiveMissionCount < 2;
    actionButton.interactable = slotFree;

    // Optional: dark-red button + tooltip when no slot
    // if (!slotFree) ShowTooltip("All mission slots are in use");
}

    }

    private void OnActionPressed()
    {
        var mgr = MissionManager.Instance;

        if (mgr.IsComplete(missionID))
            mgr.ClaimMission(missionID);
        else
            mgr.StartMission(missionID);
    }
}
