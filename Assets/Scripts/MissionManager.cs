using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    /// <summary>How many missions are currently running.</summary>
    public int ActiveMissionCount => missions.Values.Count(m => m.isActive);

    /// <summary>Start mission if a slot is free (max 2 for now). Returns true on success.</summary>
    public bool StartMission(string id)
    {
        if (!missions.TryGetValue(id, out var m))
        {
            Debug.LogWarning($"Mission '{id}' not found");
            return false;
        }

        const int maxActive = 2; // TODO: make this upgradable
        if (m.isActive || ActiveMissionCount >= maxActive)
            return false;

        m.isActive       = true;
        m.pendingRewards = 0;
        m.leftoverSteps  = 0;
        m.Save();
        return true;
    }

    /// <summary>Maximum number of stored rewards for this mission.</summary>
public int GetRewardCapacity(string id)
    => missions.TryGetValue(id, out var m)
       ? m.definition.rewardCapacity
       : 0;


    /// <summary>Claim all accumulated rewards; returns false if none available.</summary>
    public bool ClaimMission(string id)
    {
        if (!missions.TryGetValue(id, out var m) || !m.HasRewards)
            return false;

        var items = m.ClaimAllRewards();
        foreach (var item in items)
            GameManager.Instance.AddItem(item);

        // If you want to stop the mission on claim, uncomment:
        // m.isActive = false;
        // m.Save();

        return true;
    }

    /// <summary>0–1 fill of how full the reward buffer is.</summary>
    public float GetProgress01(string id)
        => missions.TryGetValue(id, out var m) ? m.ProgressNormalized : 0f;

    /// <summary>How many whole rewards are waiting to be claimed.</summary>
    public int GetPendingRewardCount(string id)
        => missions.TryGetValue(id, out var m) ? m.pendingRewards : 0;

    /// <summary>Is this mission currently running?</summary>
    public bool IsActive(string id)
        => missions.TryGetValue(id, out var m) && m.isActive;

    /// <summary>Has this mission generated any rewards?</summary>
    public bool HasRewards(string id)
        => missions.TryGetValue(id, out var m) && m.HasRewards;

    #endregion
}

[System.Serializable]
public struct MissionDefinition
{
    public string            id;               // unique key, e.g. "LoggingOne"
    public int               stepsPerReward;   // e.g. 100 steps → 1 wood
    public int               rewardCapacity;   // max stored rewards before you must claim
    public MissionDropTable  dropTable;        // what items to roll per reward
}

[System.Serializable]
public class MissionState
{
    public MissionDefinition definition;
    public string            id;

    public bool   isActive;
    public int    pendingRewards;
    public int    leftoverSteps;

    /// <summary>True if there’s anything to claim.</summary>
    public bool HasRewards => pendingRewards > 0;

    /// <summary>Fraction of rewardCapacity filled (0–1).</summary>
    public float ProgressNormalized
        => Mathf.Clamp01((float)pendingRewards / definition.rewardCapacity);

    private string Key(string suffix) => $"Mission_{id}_{suffix}";

    public static MissionState Load(MissionDefinition def)
    {
        var ms = new MissionState
        {
            definition      = def,
            id              = def.id,
        };
        ms.isActive       = PlayerPrefs.GetInt(ms.Key("Active"),  0) == 1;
        ms.pendingRewards = PlayerPrefs.GetInt(ms.Key("Pending"), 0);
        ms.leftoverSteps  = PlayerPrefs.GetInt(ms.Key("Leftover"),0);
        return ms;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(Key("Active"),   isActive   ? 1 : 0);
        PlayerPrefs.SetInt(Key("Pending"),  pendingRewards);
        PlayerPrefs.SetInt(Key("Leftover"), leftoverSteps);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Add step progress, convert into whole rewards up to capacity,
    /// and carry over any leftover steps.
    /// </summary>
    public void AddProgress(int steps)
    {
        if (!isActive) return;

        leftoverSteps += steps;

        int produced = leftoverSteps / definition.stepsPerReward;
        if (produced > 0)
        {
            int space = definition.rewardCapacity - pendingRewards;
            int toAdd = Mathf.Min(produced, space);
            pendingRewards += toAdd;
            leftoverSteps  -= toAdd * definition.stepsPerReward;
        }

        Save();
    }

    /// <summary>
    /// Roll dropTable once per pendingReward, reset buffer, but keep leftoverSteps.
    /// </summary>
    public List<Item> ClaimAllRewards()
    {
        var allItems = new List<Item>();
        for (int i = 0; i < pendingRewards; i++)
        {
            var rewards = definition.dropTable.RollRewards();
            allItems.AddRange(rewards);
        }

        pendingRewards = 0;
        Save();
        return allItems;
    }
}
