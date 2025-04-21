using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A mission that completes once the player has walked <see cref="stepTarget"/> steps.
/// Progress and active state are written to PlayerPrefs so nothing is lost across sessions.
/// </summary>
public class ResourceMission : MonoBehaviour
{
    // ──────────────── Inspector ▸ Identity & Settings ────────────────
    [Tooltip("Unique name used as the save‑file key (e.g. “Woodcutting_1k”).")]
    public string missionID = "Mission_1000Steps";

    [Tooltip("Total steps that must be walked to finish this mission.")]
    public int stepTarget = 1_000;

    // ──────────────── Runtime State (visible for debugging) ───────────
    [SerializeField] private bool  missionActive = false;
    [SerializeField] private int   stepsSoFar    = 0;

    // ──────────────── Optional Unity‑Events (hook VFX, loot, etc.) ────
    public UnityEvent onMissionStarted;
    public UnityEvent onMissionCompleted;

    // ──────────────────────────────────────────────────────────────────
    #region public API

    /// <summary>Call from a UI button to start the mission.</summary>
    public void StartMission()
    {
        if (missionActive) return;

        stepsSoFar    = 0;
        missionActive = true;

        SaveState();               // write to disk immediately
        onMissionStarted?.Invoke();
        Debug.Log($"[ResourceMission:{missionID}] begun – need {stepTarget} steps.");
    }

    /// <summary>Returns 0 → 1 progress for UI bars, etc.</summary>
    public float GetProgressNormalized()
    {
        return missionActive ? Mathf.Clamp01((float)stepsSoFar / stepTarget) : 0f;
    }

    #endregion
    // ──────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        // ① Load any saved progress.
        LoadState();

        // ② Subscribe to the step event so we get notified once per update.
        PlayerData.OnStepsAdded += HandleStepsAdded;
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= HandleStepsAdded;
        SaveState();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveState();
    }

    private void OnApplicationQuit()
    {
        SaveState();
    }

    // ──────────────── Event Handler ────────────────
    private void HandleStepsAdded(int newlyGainedSteps)
    {
        if (!missionActive) return;

        stepsSoFar += newlyGainedSteps;
        SaveState();                        // cheap, but guarantees nothing is lost

        if (stepsSoFar >= stepTarget)
            CompleteMission();
    }

    private void CompleteMission()
    {
        missionActive = false;
        SaveState();                        // write the “inactive & done” state
        onMissionCompleted?.Invoke();

        // ▸ Put your reward logic here or in the onMissionCompleted UnityEvent.
        Debug.Log($"[ResourceMission:{missionID}] complete!");
    }

    // ──────────────── Persistence ────────────────
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
}
