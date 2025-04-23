using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [SerializeField] private MissionDefinition[] missionDefinitions;

    // Runtime data (ID → state)
    private readonly Dictionary<string, MissionState> missions = new();

    //───────────────────────────────────────────────────────────────
    #region Unity lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

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
    //───────────────────────────────────────────────────────────────
    #region Step tick

    private void OnStepsAdded(int steps)
    {
        foreach (var m in missions.Values)
            if (m.isActive && !m.isComplete)
                m.AddProgress(steps);
    }

    #endregion
    //───────────────────────────────────────────────────────────────
    #region Public API

    /// <summary>How many missions are currently running.</summary>
    public int ActiveMissionCount
        => missions.Values.Count(m => m.isActive);

    /// <summary>Start mission if a slot is free (max 2). Returns true on success.</summary>
    public bool StartMission(string id)
    {
        if (!missions.TryGetValue(id, out var m))
        { Debug.LogWarning($"Mission '{id}' not found"); return false; }

        if (m.isActive)                       return false;        // already running
        if (ActiveMissionCount >= 2)          return false;        // slots full

        // Reset if it was completed/claimed earlier
        if (m.isComplete || m.isClaimed)
        {
            m.stepsSoFar = 0;
            m.isClaimed  = false;
        }

        m.isActive = true;
        m.Save();
        return true;
    }

    /// <summary>Claim rewards; frees the slot & makes mission repeatable.</summary>
    public bool ClaimMission(string id)
    {
        if (!missions.TryGetValue(id, out var m) || !m.isComplete)
            return false;

        // ▸ Grant reward
        if (m.definition.item != null)
            GameManager.Instance.AddItem(m.definition.item);

        // ▸ Reset state so the mission becomes repeatable
        m.stepsSoFar = 0;     // ←  **important line**
        m.isActive   = false;
        m.isClaimed  = false;
        m.Save();
        return true;
    }


    public float GetProgress01(string id)
        => missions.TryGetValue(id, out var m) ? m.ProgressNormalized : 0f;

    public bool IsActive  (string id) => missions.TryGetValue(id, out var m) && m.isActive;
    public bool IsComplete(string id) => missions.TryGetValue(id, out var m) && m.isComplete;
    public bool IsClaimed (string id) => missions.TryGetValue(id, out var m) && m.isClaimed;

    #endregion
}
//─────────────────────────────────────────────────────────────────
[System.Serializable]
public struct MissionDefinition
{
    public string id;        // unique key, e.g. "LoggingOne"
    public int    stepTarget;
    public Item   item;      // reward
}
//─────────────────────────────────────────────────────────────────
[System.Serializable]
public class MissionState
{
    public MissionDefinition definition;   // full definition
    public string id;
    public int    stepTarget;

    public int  stepsSoFar;
    public bool isActive;
    public bool isClaimed;

    public bool  isComplete          => stepsSoFar >= stepTarget;
    public float ProgressNormalized  => Mathf.Clamp01((float)stepsSoFar / stepTarget);

    // Persistence helpers
    private string Key(string suffix) => $"Mission_{id}_{suffix}";

    public static MissionState Load(MissionDefinition def)
    {
        var m = new MissionState
        {
            definition = def,
            id         = def.id,
            stepTarget = def.stepTarget
        };
        m.isActive   = PlayerPrefs.GetInt(m.Key("Active"),   0) == 1;
        m.isClaimed  = PlayerPrefs.GetInt(m.Key("Claimed"),  0) == 1;
        m.stepsSoFar = PlayerPrefs.GetInt(m.Key("Progress"), 0);
        return m;
    }

    public void Save()
    {
        PlayerPrefs.SetInt(Key("Active"),   isActive ? 1 : 0);
        PlayerPrefs.SetInt(Key("Claimed"),  isClaimed ? 1 : 0);
        PlayerPrefs.SetInt(Key("Progress"), stepsSoFar);
        PlayerPrefs.Save();
    }

    public void AddProgress(int steps)
    {
        stepsSoFar += steps;
        Save();
    }
}
