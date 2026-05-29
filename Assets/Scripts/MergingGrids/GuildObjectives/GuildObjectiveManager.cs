using System;
using System.Collections.Generic;
using UnityEngine;

public class GuildObjectiveManager : MonoBehaviour
{
    public static GuildObjectiveManager Instance { get; private set; }

    public List<GuildObjective> objectives = new List<GuildObjective>();

    public int currentObjectiveIndex = 0;

    public GuildObjective CurrentObjective
    {
        get
        {
            if (objectives == null || objectives.Count == 0)
                return null;

            if (currentObjectiveIndex < 0 || currentObjectiveIndex >= objectives.Count)
                return null;

            return objectives[currentObjectiveIndex];
        }
    }

    public event Action OnObjectiveUpdated;
    public event Action<GuildObjective> OnObjectiveCompleted;

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

    public void RegisterEnemyDefeated(Enemy enemy)
{
    if (enemy == null) return;

    GuildObjective objective = CurrentObjective;
    if (objective == null) return;
    if (objective.isComplete) return;

    string defeatedEnemyID = enemy.enemyID;

    Debug.Log($"[Objective] Defeated enemy ID: {defeatedEnemyID}, Required ID: {objective.requiredEnemyID}");

    if (defeatedEnemyID != objective.requiredEnemyID)
        return;

    objective.currentKills++;

    if (objective.currentKills >= objective.requiredKills)
    {
        CompleteCurrentObjective();
    }

    OnObjectiveUpdated?.Invoke();
}

    private void CompleteCurrentObjective()
    {
        GuildObjective objective = CurrentObjective;
        if (objective == null) return;

        objective.isComplete = true;

        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.totalCopper += objective.copperReward;
            PlayerData.Instance.SavePlayerData();
        }

        Debug.Log($"Objective complete: {objective.objectiveName}");

        OnObjectiveCompleted?.Invoke(objective);

        AdvanceToNextObjective();
    }

    private void AdvanceToNextObjective()
    {
        if (currentObjectiveIndex + 1 < objectives.Count)
        {
            currentObjectiveIndex++;
            Debug.Log($"New objective: {CurrentObjective.objectiveName}");
        }
        else
        {
            Debug.Log("All objectives complete.");
        }

        OnObjectiveUpdated?.Invoke();
    }
}