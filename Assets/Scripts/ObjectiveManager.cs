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
    public Action reward;  // assign at runtime
}

public class ObjectiveManager : MonoBehaviour
{
    public List<Objective> active = new();
    public static event System.Action ObjectivesChanged;

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

    void Start() { if (active.Count == 0) RollStarterSet(); ObjectivesChanged?.Invoke(); }

    void RollStarterSet()
    {
        active.Clear();
        // 3 clean, readable goals
        active.Add(MakeKillAny(10, gold: 200));
        active.Add(MakeSpendSteps(1500, token: 1));
        active.Add(MakeClearStage(GameManager.Instance.allStagesData[0].stageID, shards: 1));
    }

    // ——— progress wiring ———
    void OnStageCleared(string stageID) => Inc(ObjectiveType.ClearStage, stageID, 1);
    void OnEnemyDefeated(Enemy e)       => Inc(ObjectiveType.KillAny, "", 1);
    void OnStepsSpent(int amt)          => Inc(ObjectiveType.SpendSteps, "", amt);

    void Inc(ObjectiveType type, string arg, int amount)
    {
        foreach (var o in active)
        {
            if (o.type != type) continue;
            if (type == ObjectiveType.ClearStage && o.arg != arg) continue;
            if (o.claimed) continue;

            o.progress = Mathf.Min(o.target, o.progress + amount);
            if (o.progress >= o.target && !o.claimed)
            {
                o.claimed = true;
                o.reward?.Invoke();
                // immediately replace with a fresh one of the same kind
                Replace(o);
            }
        }
        ObjectivesChanged?.Invoke();
        // You can ping UI here to refresh bars
    }

    void Replace(Objective old)
    {
        int idx = active.IndexOf(old);
        if (idx < 0) return;
        switch (old.type)
        {
            case ObjectiveType.KillAny:     active[idx] = MakeKillAny(UnityEngine.Random.Range(8,14), gold: 250); break;
            case ObjectiveType.SpendSteps:  active[idx] = MakeSpendSteps(UnityEngine.Random.Range(1200,2200), token: 1); break;
            case ObjectiveType.ClearStage:
                // roll a currently unlocked stage
                var stage = GameManager.Instance.currentStage ?? GameManager.Instance.allStagesData[0];
                active[idx] = MakeClearStage(stage.stageID, shards: 1);
                ObjectivesChanged?.Invoke();
                break;
        }
    }

    // ——— factory helpers with rewards ———
    Objective MakeKillAny(int target, int gold)
    {
        return new Objective {
            id = "kill_any",
            type = ObjectiveType.KillAny,
            target = target,
            progress = 0,
            reward = () => GameManager.Instance.AddGold(gold)
        };
    }

    Objective MakeSpendSteps(int target, int token)
    {
        return new Objective {
            id = "spend_steps",
            type = ObjectiveType.SpendSteps,
            target = target,
            reward = () => GameManager.Instance.AddItemById("ResearchToken", token)
        };
    }

    Objective MakeClearStage(string stageID, int shards)
    {
        return new Objective {
            id = "clear_"+stageID,
            type = ObjectiveType.ClearStage,
            arg = stageID,
            target = 1,
           //TODO reward = () => GameManager.Instance.AddItemById("Some Item Name", shards)
        };
    }
}
