using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Static reference to the currently active building.
    public static Building ActiveBuilding { get; private set; }

    public string buildingName;
    public int level;
    public bool isProducing;
    
    // Steps required to produce 1 gold coin.
    public int stepsRequiredForProduction;
    
    // Track the total gold (in gold coins) produced (claimed) over time.
    public int totalResourcesProduced;
    
    // Claimable gold coins produced based on accumulated steps.
    public int resourcesProduced;
    
    // Production multiplier can be increased with upgrades.
    public float productionMultiplier = 1f;

    // Internal accumulator for steps dedicated to production.
    private int accumulatedSteps;

    /// <summary>
    /// Call this method when the building panel is opened.
    /// It sets this building as the active one.
    /// </summary>
    public void Activate()
    {
        ActiveBuilding = this;
    }

    /// <summary>
    /// Call this when the building panel is closed.
    /// </summary>
    public void Deactivate()
    {
        if (ActiveBuilding == this)
            ActiveBuilding = null;
    }

    /// <summary>
    /// Accumulates production steps. Call this from PlayerData.UpdateSteps
    /// when this building is active.
    /// </summary>
    public void AccumulateProduction(int steps)
    {
        accumulatedSteps += steps;
        // Calculate produced gold based on accumulated steps.
        int produced = Mathf.FloorToInt(accumulatedSteps / (float)stepsRequiredForProduction * productionMultiplier);
        resourcesProduced = produced;
    }

    /// <summary>
    /// Returns the number of produced (claimable) gold coins.
    /// </summary>
    public int GetProducedGold()
    {
        return Mathf.FloorToInt(accumulatedSteps / (float)stepsRequiredForProduction * productionMultiplier);
    }

    /// <summary>
    /// Claims the produced gold: converts produced gold coins into copper currency
    /// and subtracts the corresponding steps from the accumulator.
    /// </summary>
    public void ClaimProduction()
    {
        int goldToClaim = GetProducedGold();
        if (goldToClaim > 0)
        {
            // Convert gold to copper (1 Gold = 10,000 Copper).
            long copperValue = goldToClaim * CurrencyManager.COPPER_PER_GOLD;
            CurrencyManager.AddCopper(ref PlayerData.Instance.totalCopper, copperValue);

            // Update total claimed production.
            totalResourcesProduced += goldToClaim;

            // Remove the steps used for the claimed gold.
            int stepsUsed = Mathf.FloorToInt(goldToClaim * stepsRequiredForProduction / productionMultiplier);
            accumulatedSteps -= stepsUsed;
            if (accumulatedSteps < 0) accumulatedSteps = 0;

            // Recalculate claimable production.
            resourcesProduced = GetProducedGold();
        }
    }

    /// <summary>
    /// Returns a string for UI display showing how many gold coins have been produced.
    /// (For example, to show on the building panel.)
    /// </summary>
    public string GetProductionDisplay()
    {
        return GetProducedGold().ToString();
    }

    public bool IsProducing()
    {
        return isProducing;
    }
}
