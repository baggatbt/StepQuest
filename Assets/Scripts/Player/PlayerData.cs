using System;
using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour
{
    private static PlayerData _instance;
    public static PlayerData Instance => _instance;

    public int level;
    public int exp;
    public long totalCopper;          // all currency in copper
    public int inGameSteps;           // steps the player can spend
    public int baselineSteps;         // last‐seen cumulative sensor total
    public int currentSensorTotal;    // running live total
    public int currentStageIndex;
    public bool firstTimeLogin = true;

    private StepCounterController stepCounterController;
    private Building[] buildings;
    private Coroutine stepCoroutine;
    public float updateInterval = 1f; // seconds between live checks
    private int newSteps;             // steps since last live check
    private int newSensorTotal;       // fresh sensor total each tick

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeStepCounter();
            LoadPlayerData();
            buildings = FindObjectsOfType<Building>();

            if (firstTimeLogin)
            {
                HandleFirstLogin();
            }
            else
            {
                // 1) grab fresh cumulative total
                int totalSensorSteps = stepCounterController.GetTotalSteps();

                // 2) compute “offline” delta with reset-detection (Option A)
                int offlineSteps;
                if (totalSensorSteps >= baselineSteps)
                {
                    // normal case: just diff
                    offlineSteps = totalSensorSteps - baselineSteps;
                }
                else
                {
                    // sensor reset detected: credit everything since boot
                    offlineSteps = totalSensorSteps;
                }

                if (offlineSteps > 0)
                {
                    inGameSteps           += offlineSteps;
                    currentSensorTotal     = totalSensorSteps;
                    baselineSteps          = totalSensorSteps;
                    SavePlayerData();
                    OnStepsAdded?.Invoke(offlineSteps);

                    // immediately produce resources for each building
                    foreach (var b in buildings)
                        if (b.IsProducing())
                            b.AccumulateProduction(offlineSteps);
                }

                // 3) now start live updates
                stepCoroutine = StartCoroutine(RunStepRelatedFunctions());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Persist on pause/quit so baselineSteps is always up-to-date
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SavePlayerData();
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    private IEnumerator RunStepRelatedFunctions()
    {
        while (true)
        {
            UpdateSteps();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void InitializeStepCounter()
    {
        stepCounterController = StepCounterController.Instance;
        if (stepCounterController == null)
            Debug.LogError("StepCounterController not found in the scene!");
    }

    private void HandleFirstLogin()
    {
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstTimeLogin", 0);

        baselineSteps      = stepCounterController.GetTotalSteps();
        currentSensorTotal = baselineSteps;
        inGameSteps        = 0;
        SavePlayerData();

        Debug.Log("First time login handled, step data initialized.");
        stepCoroutine = StartCoroutine(RunStepRelatedFunctions());
    }

    public static event Action<int> OnStepsAdded;

    private void UpdateSteps()
    {
        newSensorTotal = stepCounterController.GetTotalSteps();
        newSteps       = newSensorTotal - currentSensorTotal;

        if (newSteps > 0)
        {
            inGameSteps        += newSteps;
            currentSensorTotal = newSensorTotal;
            OnStepsAdded?.Invoke(newSteps);

            foreach (var b in buildings)
                if (b.IsProducing())
                    b.AccumulateProduction(newSteps);

            SavePlayerData();
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetString("PlayerTotalCopper", totalCopper.ToString());
        PlayerPrefs.SetInt("InGameSteps", inGameSteps);
        PlayerPrefs.SetInt("BaselineSteps", baselineSteps);
        PlayerPrefs.SetInt("CurrentSensorTotal", currentSensorTotal);
        Debug.Log("Player data saved.");
        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        level = PlayerPrefs.GetInt("PlayerLevel", 1);

        if (!long.TryParse(PlayerPrefs.GetString("PlayerTotalCopper", "0"), out totalCopper))
            totalCopper = 0;

        inGameSteps        = PlayerPrefs.GetInt("InGameSteps", 0);
        baselineSteps      = PlayerPrefs.GetInt("BaselineSteps", 0);
        currentSensorTotal = PlayerPrefs.GetInt("CurrentSensorTotal", baselineSteps);

        Debug.Log("Player data loaded.");
    }

    public bool UseSteps(int amountToUse)
    {
        if (inGameSteps >= amountToUse)
        {
            inGameSteps -= amountToUse;
            SavePlayerData();
            return true;
        }
        return false;
    }

    public void ResetSteps()
    {
        inGameSteps = 0;
        SavePlayerData();
        Debug.Log("In-game steps have been reset to 0.");
    }

#if UNITY_EDITOR
    // Editor-only simulation of steps
    public void DebugAddSteps(int amount)
    {
        if (amount <= 0) return;
        inGameSteps += amount;
        currentSensorTotal += amount;
        OnStepsAdded?.Invoke(amount);
        foreach (var b in buildings)
            if (b != null && b.IsProducing())
                b.AccumulateProduction(amount);
        SavePlayerData();
        Debug.Log($"[PlayerData] DEBUG added {amount} steps");
    }
#endif
}
