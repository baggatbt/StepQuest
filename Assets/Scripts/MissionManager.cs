using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    [SerializeField] private MissionDefinition[] missionDefinitions;

    // Runtime data (id → state)
    private readonly Dictionary<string, MissionState> missions = new();

    #region Unity lifecycle

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
            return;
        }

        foreach (var def in missionDefinitions)
            missions[def.id] = MissionState.Load(def);

        PlayerData.OnStepsAdded += OnStepsAdded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            PlayerData.OnStepsAdded -= OnStepsAdded;
    }

    #endregion

    #region Step tick

    private void OnStepsAdded(int steps)
    {
        foreach (var m in missions.Values)
            m.AddProgress(steps);
    }

    #endregion

    #region Public API

    public int ActiveMissionCount => missions.Values.Count(m => m.isActive);

    public bool StartMission(string id)
    {
        if (!missions.TryGetValue(id, out var m))
        {
            Debug.LogWarning($"Mission '{id}' not found");
            return false;
        }

        const int maxActive = 2;
        if (m.isActive || ActiveMissionCount >= maxActive)
            return false;

        m.isActive       = true;
        m.pendingRewards = 0;
        m.leftoverSteps  = 0;
        m.Save();
        return true;
    }

    public bool ClaimMission(string id)
    {
        if (!missions.TryGetValue(id, out var m) || !m.HasRewards)
            return false;

        var items = m.ClaimAllRewards();
        foreach (var item in items)
            GameManager.Instance.AddItem(item);

        return true;
    }

    public float GetProgress01(string id)
        => missions.TryGetValue(id, out var m) ? m.ProgressNormalized : 0f;

    public int GetPendingRewardCount(string id)
        => missions.TryGetValue(id, out var m) ? m.pendingRewards : 0;

    public int GetRewardCapacity(string id)
        => missions.TryGetValue(id, out var m) ? m.definition.rewardCapacity : 0;

    public bool IsActive(string id)
        => missions.TryGetValue(id, out var m) && m.isActive;

    public bool HasRewards(string id)
        => missions.TryGetValue(id, out var m) && m.HasRewards;

    #endregion
}

[System.Serializable]
public struct MissionDefinition
{
    public string           id;               // "LoggingOne"
    public string           skillID;          // e.g. "Woodcutting"
    public int              capacityPerLevel; // extra storage per skill level
    public float            efficiencyPerLevel; // speed bonus per level
    public int              stepsPerReward;   // base steps → 1 reward
    public int              rewardCapacity;   // base storage capacity
    public int              xpPerReward;      // NEW: XP to award per reward claimed
    public MissionDropTable dropTable;
}

[System.Serializable]
public class MissionState
{
    public MissionDefinition definition;
    public string            id;

    public bool   isActive;
    public int    pendingRewards;
    public int    leftoverSteps;

    public bool HasRewards => pendingRewards > 0;

    public float ProgressNormalized
    {
        get
        {
            int lvl    = TaskSkillManager.Instance.GetLevel(definition.skillID);
            int maxCap = definition.rewardCapacity + lvl * definition.capacityPerLevel;
            return Mathf.Clamp01((float)pendingRewards / maxCap);
        }
    }

    private string Key(string suffix) => $"Mission_{id}_{suffix}";

    public static MissionState Load(MissionDefinition def)
    {
        var ms = new MissionState
        {
            definition      = def,
            id              = def.id,
            isActive        = PlayerPrefs.GetInt(msKey(def.id, "Active"),  0) == 1,
            pendingRewards  = PlayerPrefs.GetInt(msKey(def.id, "Pending"), 0),
            leftoverSteps   = PlayerPrefs.GetInt(msKey(def.id, "Leftover"),0)
        };
        return ms;
    }

    private static string msKey(string id, string suffix) => $"Mission_{id}_{suffix}";

    public void Save()
    {
        PlayerPrefs.SetInt(Key("Active"),   isActive   ? 1 : 0);
        PlayerPrefs.SetInt(Key("Pending"),  pendingRewards);
        PlayerPrefs.SetInt(Key("Leftover"), leftoverSteps);
        PlayerPrefs.Save();
    }

    public void AddProgress(int steps)
    {
        if (!isActive) return;

        int lvl       = TaskSkillManager.Instance.GetLevel(definition.skillID);
        int maxCap    = definition.rewardCapacity + lvl * definition.capacityPerLevel;
        float scale   = 1f - lvl * definition.efficiencyPerLevel;
        int effSPR    = Mathf.Max(1, Mathf.RoundToInt(definition.stepsPerReward * scale));

        leftoverSteps += steps;
        int produced  = leftoverSteps / effSPR;
        if (produced > 0)
        {
            int space = maxCap - pendingRewards;
            int toAdd = Mathf.Min(produced, space);
            pendingRewards += toAdd;
            leftoverSteps  -= toAdd * effSPR;
        }
        Save();
    }

    public List<Item> ClaimAllRewards()
    {
        // — Award XP before clearing pendingRewards —
        int totalXP = pendingRewards * definition.xpPerReward;
        TaskSkillManager.Instance.AddXP(definition.skillID, totalXP);

        // — Roll items —
        var allItems = new List<Item>();
        for (int i = 0; i < pendingRewards; i++)
            allItems.AddRange(definition.dropTable.RollRewards());

        pendingRewards = 0;
        Save();
        return allItems;
    }
}
