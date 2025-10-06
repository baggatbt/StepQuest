using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private MissionDefinition[] missionDefinitions;

    // Make the cap editable in the Inspector. Set to 1 if you want "auto-swap" behavior.
    [SerializeField] private int maxActiveMissions = 1;

    // Runtime data (id → state)
    private readonly Dictionary<string, MissionState> missions = new();

    // Snapshot key for offline step catch-up
    private const string kLastSensorKey = "Mission_LastSensorTotal";

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

        // OFFLINE GAINS: diff cumulative sensor total and apply to ACTIVE missions
        int currentSensor = PlayerPrefs.GetInt("CurrentSensorTotal", 0);
        int lastSensor    = PlayerPrefs.GetInt(kLastSensorKey, currentSensor);
        int offlineSteps  = Mathf.Max(0, currentSensor - lastSensor);

        if (offlineSteps > 0)
        {
            foreach (var m in missions.Values)
                m.AddProgress(offlineSteps);
        }

        PlayerPrefs.SetInt(kLastSensorKey, currentSensor);
        PlayerPrefs.Save();

        PlayerData.OnStepsAdded += OnStepsAdded;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SnapshotSensor();
    }

    private void OnApplicationQuit()
    {
        SnapshotSensor();
    }

    private void SnapshotSensor()
    {
        int currentSensor = PlayerPrefs.GetInt("CurrentSensorTotal", 0);
        PlayerPrefs.SetInt(kLastSensorKey, currentSensor);
        PlayerPrefs.Save();
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

    /// <summary>
    /// Start a mission if capacity allows. Returns true if started, false otherwise.
    /// </summary>
    public bool StartMission(string id)
    {
        if (!missions.TryGetValue(id, out var m))
        {
            Debug.LogWarning($"Mission '{id}' not found");
            return false;
        }

        if (m.isActive) return true; // already running

        if (ActiveMissionCount >= maxActiveMissions)
            return false; // capacity full

        m.isActive       = true;
        m.pendingRewards = m.pendingRewards; // keep
        m.leftoverSteps  = m.leftoverSteps;  // keep
        m.Save();
        return true;
    }

    /// <summary>
    /// Stop a mission if active. Returns true if stopped, false if not active/not found.
    /// </summary>
    public bool StopMission(string id)
    {
        if (!missions.TryGetValue(id, out var m) || !m.isActive)
            return false;

        m.isActive = false;
        m.Save();
        return true;
    }

    /// <summary>
    /// Start a mission; if capacity is full, stop one currently active mission (swap) and start the new one.
    /// Returns the id of any mission that was stopped (for UI), or null if none stopped.
    /// </summary>
    public string StartOrSwapMission(string id)
    {
        if (!missions.TryGetValue(id, out var target))
        {
            Debug.LogWarning($"Mission '{id}' not found");
            return null;
        }

        // If already running, nothing to do.
        if (target.isActive) return null;

        // If there is room, just start it.
        if (ActiveMissionCount < maxActiveMissions)
        {
            StartMission(id);
            return null;
        }

        // Otherwise swap: stop one active mission (choose a deterministic one)
        var activeId = GetFirstActiveMissionId(excludeId: id);
        if (activeId != null)
        {
            StopMission(activeId);
            StartMission(id);
            return activeId;
        }

        // Shouldn't hit here, but safe fallback.
        return null;
    }

    /// <summary>
    /// Returns the first active mission id (optionally excluding a specific id), or null.
    /// </summary>
    public string GetFirstActiveMissionId(string excludeId = null)
    {
        foreach (var kvp in missions)
        {
            if (kvp.Key == excludeId) continue;
            if (kvp.Value.isActive) return kvp.Key;
        }
        return null;
    }

    /// <summary>
    /// Claim pending rewards (keeps the mission's active state unchanged).
    /// </summary>
    public bool ClaimMission(string id)
    {
        if (!missions.TryGetValue(id, out var m) || !m.HasRewards)
            return false;

        var items = m.ClaimAllRewards();
        foreach (var item in items)
            GameManager.Instance.AddItem(item);

        return true;
    }

    public bool IsActive(string id)
        => missions.TryGetValue(id, out var m) && m.isActive;

    public bool HasRewards(string id)
        => missions.TryGetValue(id, out var m) && m.HasRewards;

    public float GetProgress01(string id)
        => missions.TryGetValue(id, out var m) ? m.ProgressNormalized : 0f;

    public int GetPendingRewardCount(string id)
        => missions.TryGetValue(id, out var m) ? m.pendingRewards : 0;

    public int GetRewardCapacity(string id)
    {
        if (!missions.TryGetValue(id, out var m))
        {
            Debug.LogWarning($"Mission '{id}' not found");
            return 0;
        }
        int lvl = TaskSkillManager.Instance.GetLevel(m.definition.skillID);
        return m.definition.rewardCapacity + lvl * m.definition.capacityPerLevel;
    }

    // ── Extra getters for Activity Tracker ──────────────────────────
    public int GetStepsToNextReward(string id)
        => missions.TryGetValue(id, out var m) ? m.StepsToNextReward() : 0;

    public int GetEffectiveStepsPerReward(string id)
        => missions.TryGetValue(id, out var m) ? m.EffectiveStepsPerReward() : 0;

    public int GetLeftoverSteps(string id)
        => missions.TryGetValue(id, out var m) ? m.leftoverSteps : 0;

    public int GetTotalStepsAccumulated(string id)
        => missions.TryGetValue(id, out var m) ? m.totalStepsAccumulated : 0;

    public int GetPendingUnclaimedXP(string id)
        => missions.TryGetValue(id, out var m) ? m.PendingUnclaimedXP() : 0;

    public string GetSkillID(string id)
        => missions.TryGetValue(id, out var m) ? m.definition.skillID : string.Empty;

    public MissionDefinition GetDefinition(string id)
        => missions.TryGetValue(id, out var m) ? m.definition : default;

    #endregion
}

[System.Serializable]
public struct MissionDefinition
{
    public string           id;                 // "LoggingOne"
    public string           skillID;            // e.g. "Woodcutting"
    public int              capacityPerLevel;   // extra storage per skill level
    public float            efficiencyPerLevel; // speed bonus per level
    public int              stepsPerReward;     // base steps → 1 reward
    public int              rewardCapacity;     // base storage capacity
    public int              xpPerReward;        // XP per claimed reward
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

    // Lifetime stat (optional UI)
    public int    totalStepsAccumulated;

    public bool HasRewards => pendingRewards > 0;

    // Helpers
    public int EffectiveStepsPerReward()
    {
        int lvl   = TaskSkillManager.Instance.GetLevel(definition.skillID);
        float eff = 1f + lvl * definition.efficiencyPerLevel;
        return Mathf.Max(1, Mathf.RoundToInt(definition.stepsPerReward / eff));
    }

    public int StepsToNextReward()
    {
        int spr = EffectiveStepsPerReward();
        int rem = spr - leftoverSteps;
        return Mathf.Clamp(rem, 0, spr);
    }

    public int PendingUnclaimedXP()
        => pendingRewards * definition.xpPerReward;

    public float ProgressNormalized
    {
        get
        {
            int lvl    = TaskSkillManager.Instance.GetLevel(definition.skillID);
            int maxCap = definition.rewardCapacity + lvl * definition.capacityPerLevel;
            return maxCap <= 0 ? 0f : Mathf.Clamp01((float)pendingRewards / maxCap);
        }
    }

    private string Key(string suffix) => $"Mission_{id}_{suffix}";
    private static string KeyStatic(string id, string suffix) => $"Mission_{id}_{suffix}";

    public static MissionState Load(MissionDefinition def)
    {
        return new MissionState
        {
            definition            = def,
            id                    = def.id,
            isActive              = PlayerPrefs.GetInt(KeyStatic(def.id, "Active"),   0) == 1,
            pendingRewards        = PlayerPrefs.GetInt(KeyStatic(def.id, "Pending"),  0),
            leftoverSteps         = PlayerPrefs.GetInt(KeyStatic(def.id, "Leftover"), 0),
            totalStepsAccumulated = PlayerPrefs.GetInt(KeyStatic(def.id, "Total"),    0),
        };
    }

    public void Save()
    {
        PlayerPrefs.SetInt(Key("Active"),   isActive   ? 1 : 0);
        PlayerPrefs.SetInt(Key("Pending"),  pendingRewards);
        PlayerPrefs.SetInt(Key("Leftover"), leftoverSteps);
        PlayerPrefs.SetInt(Key("Total"),    totalStepsAccumulated);
        PlayerPrefs.Save();
    }

    public void AddProgress(int steps)
    {
        if (!isActive || steps <= 0) return;

        int spr = EffectiveStepsPerReward();

        leftoverSteps         += steps;
        totalStepsAccumulated += steps;

        int produced = leftoverSteps / spr;
        if (produced > 0)
        {
            int lvl    = TaskSkillManager.Instance.GetLevel(definition.skillID);
            int maxCap = definition.rewardCapacity + lvl * definition.capacityPerLevel;
            int space  = maxCap - pendingRewards;
            int toAdd  = Mathf.Min(produced, Mathf.Max(space, 0));

            pendingRewards += toAdd;
            leftoverSteps  -= toAdd * spr;
        }

        Save();
    }

    public List<Item> ClaimAllRewards()
    {
        int totalXP = pendingRewards * definition.xpPerReward;
        TaskSkillManager.Instance.AddXP(definition.skillID, totalXP);

        var allItems = new List<Item>();
        for (int i = 0; i < pendingRewards; i++)
            allItems.AddRange(definition.dropTable.RollRewards());

        pendingRewards = 0;
        Save();
        return allItems;
    }
}


