using System.Collections.Generic;
using UnityEngine;
using System;       

/// <summary>
/// A step-driven crafting system that:
///  • Queues recipes as ActiveCraftJob objects (up to ‘MaxActiveSlots’ at once).
///  • Listens to PlayerData.OnStepsAdded to advance each job’s progress.
///  • Automatically grants output when sufficient steps accumulate (even offline).
///  • Persists all active jobs + crafting level + slot upgrades in PlayerPrefs (via JSON).
///  • Supports a “crafting level” that reduces required steps, and a “slots level” that increases max concurrent jobs.
/// </summary>
public class CraftingManager : MonoBehaviour
{
    [Header("All Recipe assets (drag all your Recipe ScriptableObjects here)")]
    public List<Recipe> allRecipes = new List<Recipe>();

    [Header("References")]
    [Tooltip("Drag your GameManager (with AddItem/HasItem/RemoveItem) here")]
    public GameManager gameManager;

    public event Action OnCraftingLevelChanged;

    // ─────────────────────────────────────────────────────────────────────────
    // Internals
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Represents one active crafting job.
    /// </summary>
    private class ActiveCraftJob
    {
        public Recipe recipe;      // which Recipe we’re working on
        public float stepProgress; // how many steps have been applied so far

        /// <summary>
        /// Constructor from serialized data.
        /// </summary>
        public ActiveCraftJob(Recipe r, float progress)
        {
            recipe = r;
            stepProgress = progress;
        }
    }

    // In-memory list of all jobs currently in progress
    private List<ActiveCraftJob> activeCrafts = new List<ActiveCraftJob>();

    // ─────────────────────────────────────────────────────────────────────────
    // Configurable “base” values (you can tweak these in Inspector or serialize)
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Crafting Level Settings (reduces step cost)")]
    [Tooltip("Each level reduces required steps by this fraction. 0.1 = 10% reduction per level")]
    [Range(0f, 0.5f)]
    public float reductionPerLevel = 0.10f;

    [Header("Crafting Slots Settings (limits concurrent jobs)")]
    [Tooltip("How many active slots you get at level 0 (unupgraded).")]
    public int baseActiveSlots = 1;

    [Tooltip("Each slots‐level increases max slots by this amount.")]
    public int slotsPerUpgrade = 1;

    // ─────────────────────────────────────────────────────────────────────────
    // Persistence Keys
    // ─────────────────────────────────────────────────────────────────────────

    private const string PREFS_JOBS_KEY      = "CraftJobsData";
    private const string PREFS_CRAFT_LVL_KEY = "CraftingLevel";
    private const string PREFS_SLOT_LVL_KEY  = "CraftingSlotsLevel";

    // ─────────────────────────────────────────────────────────────────────────
    // Current “levels” (saved in PlayerPrefs)
    // ─────────────────────────────────────────────────────────────────────────

    [SerializeField] private int craftingLevel   = 0;  // affects step‐cost reduction
    [SerializeField] private int slotsUpgradeLevel = 0; // affects max concurrent slots

    // ─────────────────────────────────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // 1) Load crafting‐level and slots‐level, then load any saved active jobs
        LoadCraftingLevel();
        LoadSlotsUpgradeLevel();
        LoadActiveJobs();
    }

    private void OnEnable()
    {
        // Subscribe so that whenever steps are added (in‐app or offline),
        // we advance each active job’s progress.
        PlayerData.OnStepsAdded += OnSteps;
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= OnSteps;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns how many steps this recipe actually requires after factoring in craftingLevel.
    /// </summary>
    public float GetStepsRequired(Recipe recipe)
    {
        float baseRequired = recipe.craftDuration;
        float factor = Mathf.Pow(1f - reductionPerLevel, craftingLevel);
        return baseRequired * factor;
    }

    /// <summary>
    /// Returns the current maximum number of active craft jobs allowed.
    /// </summary>
    public int GetMaxActiveSlots()
    {
        return baseActiveSlots + (slotsUpgradeLevel * slotsPerUpgrade);
    }

    /// <summary>
    /// Returns the number of slots currently in use.
    /// </summary>
    public int GetUsedSlots()
    {
        return activeCrafts.Count;
    }

    /// <summary>
    /// Returns the current crafting level (for UI display).
    /// </summary>
    public int GetCraftingLevel()
    {
        return craftingLevel;
    }

    /// <summary>
    /// Returns the current slots‐upgrade level (for UI display).
    /// </summary>
    public int GetSlotsUpgradeLevel()
    {
        return slotsUpgradeLevel;
    }

    /// <summary>
    /// Attempts to start a new crafting job for the given recipe.
    /// Immediately consumes materials if there’s an open slot; otherwise warns.
    /// Call this from your UI button.
    /// </summary>
    public void StartCrafting(Recipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("StartCrafting called with a null Recipe.");
            return;
        }

        // 1) Check for an available slot
        if (activeCrafts.Count >= GetMaxActiveSlots())
        {
            Debug.LogWarning($"[Crafting] All {GetMaxActiveSlots()} slots are in use. Upgrade slots to queue more crafts.");
            return;
        }

        // 2) Check if player has all required materials
        if (!HasAllMaterials(recipe))
        {
            Debug.LogWarning($"[Crafting] Not enough materials to craft: {recipe.outputItem.itemName}");
            return;
        }

        // 3) Consume those materials up front
        ConsumeMaterials(recipe);

        // 4) Enqueue a new ActiveCraftJob with stepProgress = 0
        var newJob = new ActiveCraftJob(recipe, 0f);
        activeCrafts.Add(newJob);

        // 5) Persist updated job list
        SaveActiveJobs();

        Debug.Log($"[Crafting] Started '{recipe.outputItem.itemName}'. " +
                  $"Needs {GetStepsRequired(recipe)} steps. (Slots used: {activeCrafts.Count}/{GetMaxActiveSlots()})");
    }

    /// <summary>
    /// Attempts to raise craftingLevel by ‘delta’. Player must handle resource cost separately.
    /// Example: if player has enough IronBars, they call RemoveItem(ironBar, cost) then IncreaseCraftingLevel(1).
    /// </summary>
    public void IncreaseCraftingLevel(int delta = 1)
    {
        craftingLevel = Mathf.Max(0, craftingLevel + delta);
        SaveCraftingLevel();
        Debug.Log($"[Crafting] Crafting level is now {craftingLevel}.");

        // 2) Fire the event here:
        OnCraftingLevelChanged?.Invoke();
    }

    /// <summary>
    /// Attempts to raise slotsUpgradeLevel by ‘delta’. Player must handle resource cost separately.
    /// </summary>
    public void IncreaseSlotsUpgradeLevel(int delta = 1)
    {
        slotsUpgradeLevel = Mathf.Max(0, slotsUpgradeLevel + delta);
        SaveSlotsUpgradeLevel();
        Debug.Log($"[Crafting] Slots‐upgrade level is now {slotsUpgradeLevel} (MaxSlots = {GetMaxActiveSlots()}).");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Step Tick Handler
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by PlayerData whenever steps are added (in-app or offline catch-up).
    /// Distributes 'added' steps to every active craft job. If a job crosses its
    /// required‐steps threshold, we grant output and remove it.
    /// </summary>
    private void OnSteps(int added)
    {
        if (added <= 0 || activeCrafts.Count == 0)
            return;

        // Collect any jobs that finish this tick so we can remove them afterward
        List<ActiveCraftJob> completedJobs = new List<ActiveCraftJob>();

        foreach (var job in activeCrafts)
        {
            job.stepProgress += added;
            float needed = GetStepsRequired(job.recipe);

            // If we've reached or exceeded needed steps, complete the craft
            if (job.stepProgress >= needed)
            {
                // 1) Grant the output items
                for (int i = 0; i < job.recipe.outputQuantity; i++)
                {
                    gameManager.AddItem(job.recipe.outputItem);
                    TaskSkillManager.Instance.AddXP("Crafting", job.recipe.expForCraft);//TODO HARD CODED 1XP, SHOULD COME FROM RECIPE
                }

                Debug.Log($"[Crafting] Completed '{job.recipe.outputItem.itemName}' ×{job.recipe.outputQuantity}.");

                // 2) Mark for removal
                completedJobs.Add(job);
            }
        }

        // Remove completed jobs from activeCrafts
        foreach (var job in completedJobs)
            activeCrafts.Remove(job);

        // If any jobs finished, persist the new job list
        if (completedJobs.Count > 0)
            SaveActiveJobs();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Checks inventory for all MaterialItem requirements of this recipe.
    /// </summary>
    private bool HasAllMaterials(Recipe recipe)
    {
        foreach (var req in recipe.materialRequirements)
        {
            if (!gameManager.HasItem(req.material, req.quantity))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Removes each required MaterialItem from inventory.
    /// </summary>
    private void ConsumeMaterials(Recipe recipe)
    {
        foreach (var req in recipe.materialRequirements)
        {
            gameManager.RemoveItem(req.material, req.quantity);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Persistence: Active Jobs
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// A serializable data container for saving/loading ActiveCraftJob.
    /// </summary>
    [System.Serializable]
    private class ActiveCraftJobData
    {
        public string recipeName;
        public float stepProgress;
    }

    /// <summary>
    /// Wrapper to allow List<ActiveCraftJobData> to be JSON‐serialized by JsonUtility.
    /// </summary>
    [System.Serializable]
    private class ActiveCraftJobDataList
    {
        public List<ActiveCraftJobData> jobs = new List<ActiveCraftJobData>();
    }

    /// <summary>
    /// Saves the current activeCrafts list into PlayerPrefs as JSON.
    /// </summary>
    private void SaveActiveJobs()
    {
        var wrapper = new ActiveCraftJobDataList();

        foreach (var job in activeCrafts)
        {
            wrapper.jobs.Add(new ActiveCraftJobData
            {
                recipeName = job.recipe.name,   // use asset name as unique identifier
                stepProgress = job.stepProgress
            });
        }

        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PREFS_JOBS_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads activeCrafts from PlayerPrefs. If no data exists, does nothing.
    /// </summary>
    private void LoadActiveJobs()
    {
        activeCrafts.Clear();

        if (!PlayerPrefs.HasKey(PREFS_JOBS_KEY))
            return;

        string json = PlayerPrefs.GetString(PREFS_JOBS_KEY);
        if (string.IsNullOrEmpty(json))
            return;

        ActiveCraftJobDataList wrapper = JsonUtility.FromJson<ActiveCraftJobDataList>(json);
        if (wrapper == null || wrapper.jobs == null)
            return;

        foreach (var data in wrapper.jobs)
        {
            // Find the matching Recipe asset by name
            Recipe found = allRecipes.Find(r => r.name == data.recipeName);
            if (found != null)
            {
                activeCrafts.Add(new ActiveCraftJob(found, data.stepProgress));
            }
            else
            {
                Debug.LogWarning($"[Crafting] Could not find Recipe named '{data.recipeName}' when loading saved jobs.");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Persistence: Crafting Level
    // ─────────────────────────────────────────────────────────────────────────

    private void SaveCraftingLevel()
    {
        PlayerPrefs.SetInt(PREFS_CRAFT_LVL_KEY, craftingLevel);
        PlayerPrefs.Save();
    }

    private void LoadCraftingLevel()
    {
        craftingLevel = PlayerPrefs.GetInt(PREFS_CRAFT_LVL_KEY, 0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Persistence: Slots Upgrade Level
    // ─────────────────────────────────────────────────────────────────────────

    private void SaveSlotsUpgradeLevel()
    {
        PlayerPrefs.SetInt(PREFS_SLOT_LVL_KEY, slotsUpgradeLevel);
        PlayerPrefs.Save();
    }

    private void LoadSlotsUpgradeLevel()
    {
        slotsUpgradeLevel = PlayerPrefs.GetInt(PREFS_SLOT_LVL_KEY, 0);
    }
}
