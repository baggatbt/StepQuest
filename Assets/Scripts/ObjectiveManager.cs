using UnityEngine;
using System;
using System.Collections.Generic;

public enum ObjectiveType { ClearStage, KillAny, SpendSteps }

[Serializable]
public class Objective
{
    public string id;
    public ObjectiveType type;
    public string arg;     // e.g., stageID for ClearStage
    public int target;
    public int progress;
    public bool claimed;
    public Action reward;  // assigned at runtime
}

public class ObjectiveManager : MonoBehaviour
{
    // Active, runtime objectives (not serialized rewards)
    public List<Objective> active = new();

    // UI can subscribe to this to refresh lists/bars
    public static event System.Action ObjectivesChanged;

    // Editor-authored objective assets (optional)
    public List<ObjectiveAsset> startingObjectiveAssets = new();

    private void OnEnable()
    {
        GameEvents.StageCleared += OnStageCleared;
        GameEvents.EnemyDefeated += OnEnemyDefeated;
        GameEvents.StepsSpent += OnStepsSpent;
    }

    private void OnDisable()
    {
        GameEvents.StageCleared -= OnStageCleared;
        GameEvents.EnemyDefeated -= OnEnemyDefeated;
        GameEvents.StepsSpent -= OnStepsSpent;
    }

    private void Start()
    {
        active.Clear();

        // Prefer authored assets if present
        if (startingObjectiveAssets != null && startingObjectiveAssets.Count > 0)
        {
            foreach (var asset in startingObjectiveAssets)
            {
                if (asset != null) AddObjective(asset.ToRuntime());
            }
        }
        else
        {
            // Fallback starter set
            RollStarterSet();
        }

        ObjectivesChanged?.Invoke();
    }

    private void RollStarterSet()
    {
        active.Clear();
        // 3 clean, readable goals
        AddObjective(MakeKillAny(10, gold: 200));
        AddObjective(MakeSpendSteps(1500, token: 1));
        // Use first stage as example; tweak as needed
        if (GameManager.Instance != null && GameManager.Instance.allStagesData.Count > 0)
        {
            var firstStageId = GameManager.Instance.allStagesData[0].stageID;
            AddObjective(MakeClearStage(firstStageId, goldReward: 300));
        }
    }

    // ─── Progress wiring ────────────────────────────────────────────────────────
    private void OnStageCleared(string stageID) => Inc(ObjectiveType.ClearStage, stageID, 1);
    private void OnEnemyDefeated(Enemy e)       => Inc(ObjectiveType.KillAny, "", 1);
    private void OnStepsSpent(int amt)          => Inc(ObjectiveType.SpendSteps, "", amt);

    private void Inc(ObjectiveType type, string arg, int amount)
    {
        bool changed = false;

        for (int i = 0; i < active.Count; i++)
        {
            var o = active[i];
            if (o.type != type) continue;
            if (type == ObjectiveType.ClearStage && o.arg != arg) continue;
            if (o.claimed) continue;

            o.progress = Mathf.Min(o.target, o.progress + amount);
            changed = true;

            if (o.progress >= o.target && !o.claimed)
            {
                o.claimed = true;
                o.reward?.Invoke();
                // Immediately replace with a fresh one of the same kind
                active[i] = Replace(o);
            }
        }

        if (changed) ObjectivesChanged?.Invoke();
    }

    private Objective Replace(Objective old)
    {
        switch (old.type)
        {
            case ObjectiveType.KillAny:
                return MakeKillAny(UnityEngine.Random.Range(8, 14), gold: 250);

            case ObjectiveType.SpendSteps:
                return MakeSpendSteps(UnityEngine.Random.Range(1200, 2200), token: 1);

            case ObjectiveType.ClearStage:
                // Roll current or first stage
                var stage = GameManager.Instance.currentStage ??
                            (GameManager.Instance.allStagesData.Count > 0 ? GameManager.Instance.allStagesData[0] : null);
                var stageId = stage != null ? stage.stageID : old.arg;
                return MakeClearStage(stageId, goldReward: 300);
        }

        // Fallback: return the old if type unknown
        return old;
    }

    // Public add helper (ensures UI refresh + logging)
    public void AddObjective(Objective o)
    {
        if (o == null) return;
        active.Add(o);
        ObjectivesChanged?.Invoke();
        Debug.Log($"[Objectives] Added: {o.type} target={o.target} arg={o.arg}");
    }

    // ─── Factory helpers with rewards ──────────────────────────────────────────
    private Objective MakeKillAny(int target, int gold)
    {
        return new Objective
        {
            id = "kill_any",
            type = ObjectiveType.KillAny,
            target = Mathf.Max(1, target),
            progress = 0,
            reward = () => GameManager.Instance.AddGold(gold)
        };
    }

    private Objective MakeSpendSteps(int target, int token)
    {
        return new Objective
        {
            id = "spend_steps",
            type = ObjectiveType.SpendSteps,
            target = Mathf.Max(1, target),
            progress = 0,
            reward = () => GameManager.Instance.AddItemById("ResearchToken", token)
        };
    }

    private Objective MakeClearStage(string stageID, int goldReward)
    {
        return new Objective
        {
            id = "clear_" + stageID,
            type = ObjectiveType.ClearStage,
            arg = stageID,
            target = 1,
            progress = 0,
            reward = () => GameManager.Instance.AddGold(goldReward)
        };
    }
}
