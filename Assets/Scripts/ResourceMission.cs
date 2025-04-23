/*
// Assets/Scripts/Missions/ResourceMission.cs
using UnityEngine;
using UnityEngine.Events;

/// <summary>Completes once the player has walked <see cref="stepTarget"/> steps.</summary>
public class ResourceMission : MonoBehaviour
{
    // --------------- Inspector ----------------
    [Tooltip("Unique key used for saving progress.  MUST be unique.")]
    public string missionID  = "LoggingOne";
    public int    stepTarget = 1_000;

    // --------------- Runtime state ------------
    [SerializeField] private bool missionActive = false;
    [SerializeField] private int  stepsSoFar    = 0;

    // --------------- Events -------------------
    public UnityEvent onMissionStarted;
    public UnityEvent onMissionCompleted;

    // --------------- Public read-only helpers --
    public bool  IsActive         => missionActive;
    public bool  IsComplete       => !missionActive && stepsSoFar >= stepTarget;
    public float ProgressNormalized => Mathf.Clamp01((float)stepsSoFar / stepTarget);

    // --------------------------------------------------------------------
    #region Life-cycle / registration

    private void Awake()
{
    // Ensure a manager exists, even if someone forgot to place it
    if (MissionManager.Instance == null)
        new GameObject("MissionManager").AddComponent<MissionManager>();

    MissionManager.Instance.Register(this);
}


    private void OnEnable()
    {
        LoadState();
        PlayerData.OnStepsAdded += HandleStepsAdded;
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= HandleStepsAdded;
        SaveState();
    }

    public void StartMissionFromButton()
    {
        TryStartMission();
    }

    private void OnDestroy()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.Unregister(this);
    }

    private void OnApplicationPause(bool pause) { if (pause) SaveState(); }
    private void OnApplicationQuit()            { SaveState(); }

    #endregion
    // --------------------------------------------------------------------
    #region Public API

    /// <summary>UI calls this. Returns <c>true</c> if the mission actually began.</summary>
    public bool TryStartMission()
    {
        if (missionActive || IsComplete)
            return false;                                     // already running or done

        if (!MissionManager.Instance.CanStartNewMission())
        {
            Debug.Log($"[Mission:{missionID}] Cannot start – limit reached.");
            return false;
        }

        stepsSoFar    = 0;
        missionActive = true;

        SaveState();
        onMissionStarted?.Invoke();
        Debug.Log($"[Mission:{missionID}] started (need {stepTarget} steps)");
        return true;
    }

    #endregion
    // --------------------------------------------------------------------
    #region Step event & completion

    private void HandleStepsAdded(int newSteps)
    {
        if (!missionActive) return;

        stepsSoFar += newSteps;
        SaveState();

        if (stepsSoFar >= stepTarget)
            CompleteMission();
    }

    private void CompleteMission()
    {
        missionActive = false;
        SaveState();
        onMissionCompleted?.Invoke();
        Debug.Log($"[Mission:{missionID}] COMPLETE!");
    }

    #endregion
    // --------------------------------------------------------------------
    #region Persistence

    private string KeyActive   => $"{missionID}_Active";
    private string KeyProgress => $"{missionID}_Steps";

    private void SaveState()
    {
        PlayerPrefs.SetInt(KeyActive,   missionActive ? 1 : 0);
        PlayerPrefs.SetInt(KeyProgress, stepsSoFar);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        missionActive = PlayerPrefs.GetInt(KeyActive,   0) == 1;
        stepsSoFar    = PlayerPrefs.GetInt(KeyProgress, 0);
    }

    #endregion
}
*/