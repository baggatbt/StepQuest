using System;
using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Objective", fileName = "New Objective")]
public class ObjectiveAsset : ScriptableObject
{
    public ObjectiveType type = ObjectiveType.KillAny;

    [Header("Target")]
    public int target = 10;             // e.g., kills/steps or 1 for ClearStage
    public StageData stage;             // used only when type == ClearStage

    [Header("Reward")]
    public int rewardGold = 0;          // set a gold reward in Inspector
    public Item rewardItem;             // optional item reward (drag your Item)
    public int rewardItemCount = 1;

    // Convert to the runtime Objective used by ObjectiveManager
    public Objective ToRuntime()
    {
        var o = new Objective
        {
            id       = $"{name}_{Guid.NewGuid():N}",
            type     = type,
            arg      = (type == ObjectiveType.ClearStage && stage != null) ? stage.stageID : string.Empty,
            target   = Mathf.Max(1, target),
            progress = 0,
            reward   = () =>
            {
                if (rewardGold > 0) GameManager.Instance.AddGold(rewardGold);
                if (rewardItem != null && rewardItemCount > 0)
                    GameManager.Instance.AddItemById(rewardItem.itemID.ToString(), rewardItemCount);
            }
        };
        return o;
    }
}
