using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A step-driven crafting system that:
///  • Queues recipes as ActiveCraftJob batches (up to ‘MaxActiveSlots’ at once).
///  • Listens to PlayerData.OnStepsAdded to advance each job’s progress.
///  • Automatically grants output when sufficient steps accumulate, handling batch quantities.
///  • Persists active jobs + crafting level + slot upgrades via PlayerPrefs (JSON).
///  • Supports a "crafting level" that reduces required steps, and a "slots level" that increases max concurrent slots.
/// </summary>
public class CraftingManager : MonoBehaviour
{
    [Header("All Recipe assets (drag your Recipe ScriptableObjects here)")]
    public List<Recipe> allRecipes = new List<Recipe>();

    [Header("References")]
    [Tooltip("Drag your GameManager (with AddItem/HasItem/RemoveItem) here")]
    public GameManager gameManager;

    // Events
    public event Action OnCraftingLevelChanged;
    public event Action OnActiveJobsChanged;

    // Public API for UI
    /// <summary>Current crafting level (for display).</summary>
    public int GetCraftingLevel()       => craftingLevel;
    /// <summary>Current slots-upgrade level (for display).</summary>
    public int GetSlotsUpgradeLevel()   => slotsUpgradeLevel;
    /// <summary>Current number of active slots in use.</summary>
    public int GetUsedSlots()           => activeCrafts.Count;
    /// <summary>Maximum concurrent slots allowed.</summary>
    public int GetMaxActiveSlots()      => baseActiveSlots + slotsUpgradeLevel * slotsPerUpgrade;
    /// <summary>Returns how many steps this recipe requires at current craftingLevel.</summary>
    public float GetStepsRequired(Recipe recipe)
    {
        float baseRequired = recipe.craftDuration;
        float factor       = Mathf.Pow(1f - reductionPerLevel, craftingLevel);
        return baseRequired * factor;
    }

    /// <summary>Recipe for slot #i.</summary>
    public Recipe GetActiveRecipe(int i)          => activeCrafts[i].recipe;
    /// <summary>Quantity remaining in batch for slot #i.</summary>
    public int    GetActiveJobQuantity(int i)     => activeCrafts[i].quantity;
    /// <summary>Progress toward next item in slot #i.</summary>
    public float  GetActiveJobProgress(int i)     => activeCrafts[i].stepProgress;

    // ─────────────────────────────────────────────────────────────────────────
    // Internals
    // ─────────────────────────────────────────────────────────────────────────

    [Serializable]
    private class ActiveCraftJob
    {
        public Recipe recipe;
        public int    quantity;
        public float  stepProgress;

        public ActiveCraftJob(Recipe recipe, int quantity, float initialProgress)
        {
            this.recipe       = recipe;
            this.quantity     = quantity;
            this.stepProgress = initialProgress;
        }
    }

    private List<ActiveCraftJob> activeCrafts = new List<ActiveCraftJob>();

    [Header("Crafting Level Settings (reduces step cost)")]
    [Tooltip("Each level reduces required steps by this fraction. 0.1 = 10% per level")]
    [Range(0f, 0.5f)]
    public float reductionPerLevel = 0.10f;

    [Header("Crafting Slots Settings (limits concurrent batches)")]
    [Tooltip("Base number of slots at level 0.")]
    public int baseActiveSlots = 1;
    [Tooltip("Additional slots per upgrade level.")]
    public int slotsPerUpgrade = 1;

    // Persistence keys
    private const string PREFS_JOBS_KEY      = "CraftJobsData";
    private const string PREFS_CRAFT_LVL_KEY = "CraftingLevel";
    private const string PREFS_SLOT_LVL_KEY  = "CraftingSlotsLevel";

    [SerializeField] private int craftingLevel     = 0;
    [SerializeField] private int slotsUpgradeLevel = 0;

    private void Awake()
    {
        LoadCraftingLevel();
        LoadSlotsUpgradeLevel();
        LoadActiveJobs();
    }

    private void OnEnable()
    {
        PlayerData.OnStepsAdded += OnSteps;
    }

    private void OnDisable()
    {
        PlayerData.OnStepsAdded -= OnSteps;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Starting a batch
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Begin crafting 'quantity' copies of this recipe in one slot.
    /// Consumes all materials up front.
    /// </summary>
    public void StartCrafting(Recipe recipe, int quantity)
    {
        if (activeCrafts.Count >= GetMaxActiveSlots())
        {
            Debug.LogWarning($"[Crafting] All {GetMaxActiveSlots()} slots in use.");
            return;
        }

        // 1) check materials
        foreach (var req in recipe.materialRequirements)
        {
            int have = gameManager.GetItemCount(req.material);
            int need = req.quantity * quantity;
            if (have < need)
            {
                Debug.LogWarning($"[Crafting] Not enough {req.material.itemName}. Need {need}, have {have}.");
                return;
            }
        }
        // 2) consume materials
        foreach (var req in recipe.materialRequirements)
            gameManager.RemoveItem(req.material, req.quantity * quantity);

        // 3) enqueue batch
        activeCrafts.Add(new ActiveCraftJob(recipe, quantity, 0f));
        SaveActiveJobs();
        OnActiveJobsChanged?.Invoke();

        Debug.Log($"[Crafting] Started '{recipe.outputItem.itemName}' ×{quantity} in 1 slot.");
    }

    /// <summary>Convenience: queue 1 if you only call single-arg.</summary>
    public void StartCrafting(Recipe recipe) => StartCrafting(recipe, 1);

    // ─────────────────────────────────────────────────────────────────────────
    // Leveling and Slots Upgrades
    // ─────────────────────────────────────────────────────────────────────────

    public void IncreaseCraftingLevel(int delta = 1)
    {
        craftingLevel = Mathf.Max(0, craftingLevel + delta);
        SaveCraftingLevel();
        OnCraftingLevelChanged?.Invoke();
        Debug.Log($"[Crafting] Level now {craftingLevel}.");
    }

    public void IncreaseSlotsUpgradeLevel(int delta = 1)
    {
        slotsUpgradeLevel = Mathf.Max(0, slotsUpgradeLevel + delta);
        SaveSlotsUpgradeLevel();
        OnCraftingLevelChanged?.Invoke();
        Debug.Log($"[Crafting] Slots-level now {slotsUpgradeLevel} (Max={GetMaxActiveSlots()}).");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Step Tick Handler
    // ─────────────────────────────────────────────────────────────────────────

    private void OnSteps(int added)
    {
        if (added <= 0 || activeCrafts.Count == 0) return;

        bool changed = false;
        
        // iterate backwards for safe removal
        for (int i = activeCrafts.Count - 1; i >= 0; i--)
        {
            var job = activeCrafts[i];
            job.stepProgress += added;
            float needed = GetStepsRequired(job.recipe);

            // how many items completed this tick?
            int done = Mathf.FloorToInt(job.stepProgress / needed);
            if (done > 0)
            {
                int take = Mathf.Min(done, job.quantity);
                for (int k = 0; k < take; k++)
                {
                    gameManager.AddItem(job.recipe.outputItem);
                    TaskSkillManager.Instance.AddXP(
                        job.recipe.skillID, job.recipe.expForCraft
                    );
                }
                job.quantity     -= take;
                job.stepProgress -= take * needed;
                changed = true;
            }

            if (job.quantity <= 0)
            {
                activeCrafts.RemoveAt(i);
                changed = true;
            }
        }

        if (changed)
        {
            SaveActiveJobs();
            OnActiveJobsChanged?.Invoke();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Persistence: Active Jobs
    // ─────────────────────────────────────────────────────────────────────────

    [Serializable]
    private class ActiveCraftJobData
    {
        public string recipeName;
        public int    quantity;
        public float  stepProgress;
    }
    
    [Serializable]
    private class ActiveCraftJobDataList { public List<ActiveCraftJobData> jobs = new List<ActiveCraftJobData>(); }

    private void SaveActiveJobs()
    {
        var wrapper = new ActiveCraftJobDataList();
        foreach (var job in activeCrafts)
        {
            wrapper.jobs.Add(new ActiveCraftJobData
            {
                recipeName   = job.recipe.name,
                quantity     = job.quantity,
                stepProgress = job.stepProgress
            });
        }
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PREFS_JOBS_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadActiveJobs()
    {
        activeCrafts.Clear();
        if (!PlayerPrefs.HasKey(PREFS_JOBS_KEY)) return;

        string json = PlayerPrefs.GetString(PREFS_JOBS_KEY);
        if (string.IsNullOrEmpty(json)) return;

        var wrapper = JsonUtility.FromJson<ActiveCraftJobDataList>(json);
        if (wrapper?.jobs == null) return;

        foreach (var d in wrapper.jobs)
        {
            var found = allRecipes.Find(r => r.name == d.recipeName);
            if (found != null)
                activeCrafts.Add(new ActiveCraftJob(found, d.quantity, d.stepProgress));
            else
                Debug.LogWarning($"[Crafting] Missing recipe '{d.recipeName}' on load.");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Persistence: Levels
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
