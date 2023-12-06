using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BattleConfig CurrentBattleConfig { get; set; }
    public HashSet<string> UnlockedStageNames = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockConnectedStages(Stage completedStage)
{
    Debug.Log($"Unlocking stages connected to: {completedStage.stageID}");

    foreach (Stage connectedStage in completedStage.connectedStages)
    {
        Debug.Log($"Checking connected stage: {connectedStage.stageID}");

        if (!UnlockedStageNames.Contains(connectedStage.stageID))
        {
            Debug.Log($"Unlocking connected stage: {connectedStage.stageID}");
            UnlockedStageNames.Add(connectedStage.stageID);
        }
        else
        {
            Debug.Log($"Stage already unlocked: {connectedStage.stageID}");
        }
    }
}

}
