using TMPro;
using UnityEngine;

public class GuildObjectiveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text rewardText;

    private void OnEnable()
    {
        if (GuildObjectiveManager.Instance != null)
        {
            GuildObjectiveManager.Instance.OnObjectiveUpdated += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (GuildObjectiveManager.Instance != null)
        {
            GuildObjectiveManager.Instance.OnObjectiveUpdated -= Refresh;
        }
    }

    public void Refresh()
    {
        GuildObjective objective = GuildObjectiveManager.Instance?.CurrentObjective;

        if (objective == null)
        {
            objectiveNameText.text = "No Objective";
            descriptionText.text = "";
            progressText.text = "";
            rewardText.text = "";
            return;
        }

        objectiveNameText.text = objective.objectiveName;
        descriptionText.text = objective.description;
        progressText.text = objective.ProgressText();
        rewardText.text = $"Reward: {objective.copperReward} copper";
    }
}