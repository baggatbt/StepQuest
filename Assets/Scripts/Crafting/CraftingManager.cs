using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    // Reference to GameManager for inventory operations
    public GameManager gameManager;
    
    // This dictionary keeps track of which item is being crafted per timer
    private Dictionary<string, CraftableItem> activeCrafts = new Dictionary<string, CraftableItem>();
    
    private void Start()
    {
        // Subscribe to the TimerCompleted event in TimerManager
        TimerManager.Instance.OnTimerCompleted += HandleTimerCompleted;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerCompleted -= HandleTimerCompleted;
        }
    }

    /// <summary>
    /// This is called by a Craft button in the UI. 
    /// Pass in the CraftableItem you want to craft.
    /// </summary>
    public void OnCraftButtonClicked(CraftableItem craftableItem)
    {
        // 1) Check material requirements
        if (!HasAllMaterials(craftableItem))
        {
            Debug.LogWarning("Not enough materials to craft: " + craftableItem.itemName);
            return;
        }

        // 2) Remove materials from inventory
        ConsumeMaterials(craftableItem);

        // 3) Start the timer
        string timerId = GenerateCraftTimerId(craftableItem);
        // For example, let’s say the duration is in the item’s stepCostToProduce, or 
        // you could store a separate craftTimeInSeconds in the item if you want.
        float craftDuration = craftableItem.stepCostToProduce; 
        TimerManager.Instance.SetTimer(timerId, craftDuration);

        // 4) Store which item is being crafted
        activeCrafts[timerId] = craftableItem;
        
        Debug.Log($"Started crafting {craftableItem.itemName} for {craftDuration} seconds.");
    }

    /// <summary>
    /// Called by the TimerManager when a timer completes.
    /// </summary>
    private void HandleTimerCompleted(string timerId)
    {
        // Check if this timerId is one of our crafting timers
        if (activeCrafts.ContainsKey(timerId))
        {
            CraftableItem craftableItem = activeCrafts[timerId];
            activeCrafts.Remove(timerId);

            // 5) When the timer completes, add the crafted item
            gameManager.AddItem(craftableItem);

            Debug.Log($"Crafting complete. {craftableItem.itemName} added to inventory.");
        }
    }

    /// <summary>
    /// Check if the player has all needed materials in their inventory.
    /// </summary>
    private bool HasAllMaterials(CraftableItem craftableItem)
    {
        // craftableItem.materialRequirements is a list of (material, quantity).
        foreach (var requirement in craftableItem.materialRequirements)
        {
            if (!gameManager.HasItem(requirement.material, requirement.quantity))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Removes the required materials from the player's inventory.
    /// </summary>
    private void ConsumeMaterials(CraftableItem craftableItem)
    {
        foreach (var requirement in craftableItem.materialRequirements)
        {
            gameManager.RemoveItem(requirement.material, requirement.quantity);
        }
    }

    /// <summary>
    /// Generates a unique ID for the crafting timer, in case you have multiple crafts going on.
    /// </summary>
    private string GenerateCraftTimerId(CraftableItem item)
    {
        // For example: "craft_{itemID}_{TimeSinceStartup}" 
        // or any other unique format that helps differentiate parallel crafts
        return $"Craft_{item.itemID}_{Time.time}";
    }
}
