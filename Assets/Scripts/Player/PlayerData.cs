using System;
using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour
{
    private static PlayerData _instance;
    public static PlayerData Instance => _instance;

    public int level;
    public int exp;

    // Store all currency in copper.
    public long totalCopper;

    public int inGameSteps;
    public int baselineSteps;
    public int currentSensorTotal;
    public int currentStageIndex;
    public bool firstTimeLogin = true;

    private StepCounterController stepCounterController;
    private Building[] buildings;
    private Coroutine stepCoroutine;
    public float updateInterval = 1f; // Interval in seconds for step-related updates
    public int newSteps;      // To track new steps
    public int newSensorTotal; // Compared against old total to see if player moved.

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
                currentSensorTotal = stepCounterController.GetTotalSteps();
                // ProduceOfflineResources();
                UpdateSteps();
            }

            // Start the coroutine to update steps and resources at intervals
            stepCoroutine = StartCoroutine(RunStepRelatedFunctions());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RunStepRelatedFunctions()
    {
        while (true)
        {
            UpdateSteps();
            // ProduceResourcesDuringGameplay();

            yield return new WaitForSeconds(updateInterval);
        }
    }

    

    private void InitializeStepCounter()
    {
        stepCounterController = StepCounterController.Instance;
        if (stepCounterController == null)
        {
            Debug.LogError("StepCounterController not found in the scene!");
        }
    }

    private void HandleFirstLogin()
    {
        firstTimeLogin = false;
        PlayerPrefs.SetInt("FirstTimeLogin", firstTimeLogin ? 1 : 0);

        baselineSteps = stepCounterController.GetTotalSteps();
        PlayerPrefs.SetInt("BaselineSteps", baselineSteps);

        inGameSteps = 0;
        currentSensorTotal = baselineSteps;
        SavePlayerData();

        Debug.Log("First time login handled, step data initialized.");
    }

    private void UpdateSteps()
{
    newSensorTotal = stepCounterController.GetTotalSteps();
    newSteps = newSensorTotal - currentSensorTotal;

    if (newSteps > 0)
    {
        inGameSteps += newSteps;
        currentSensorTotal = newSensorTotal;

        // Tell each building to produce resources
        foreach (Building building in buildings)
        {
            if (building.IsProducing())  // your building “active” logic
            {
                building.AccumulateProduction(newSteps);
            }
        }

        SavePlayerData();
    }
}



    private void ProduceOfflineResources()
    {
        int stepsSinceLastSession = currentSensorTotal - baselineSteps;
        foreach (Building building in buildings)
        {
            if (building.IsProducing())
            {
                // building.Produce(stepsSinceLastSession);
            }
        }

        baselineSteps = currentSensorTotal;
        SavePlayerData();
    }

    private void ProduceResourcesDuringGameplay()
    {
        if (newSteps > 0) // Only produce resources if there are new steps
        {
            foreach (Building building in buildings)
            {
                if (building.IsProducing())
                {
                    // building.Produce(newSteps);
                }
            }
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);

        // Store totalCopper as a string because PlayerPrefs doesn't support long directly.
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

        // Retrieve totalCopper from string
        string copperString = PlayerPrefs.GetString("PlayerTotalCopper", "0");
        long parsed;
        if (long.TryParse(copperString, out parsed))
        {
            totalCopper = parsed;
        }
        else
        {
            totalCopper = 0;
        }

        inGameSteps = PlayerPrefs.GetInt("InGameSteps", 0);
        baselineSteps = PlayerPrefs.GetInt("BaselineSteps", 0);
        currentSensorTotal = PlayerPrefs.GetInt("CurrentSensorTotal", 0);

        Debug.Log("Player data loaded.");
    }

    /// <summary>
    /// Spend in-game steps for some action (not related to currency).
    /// </summary>
    public bool UseSteps(int amountToUse)
    {
        if (inGameSteps >= amountToUse)
        {
            inGameSteps -= amountToUse;
            Debug.Log("Spent steps: " + amountToUse);
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

    /// <summary>
    /// Returns the player's currency breakdown as a multi-line string.
    /// (This can be used by UI elements if desired.)
    /// </summary>
    public string GetDisplayCurrency()
    {
        return CurrencyManager.GetMultiCoinString(totalCopper);
    }
}
