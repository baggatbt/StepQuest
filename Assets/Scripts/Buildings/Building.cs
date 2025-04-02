using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Production Settings")]
    [Tooltip("Resource produced per step.")]
    public float resourcesPerStep = 0.1f;

    [Header("Current Resource Storage")]
    public float uncollectedResources;

    /// <summary>
    /// Accumulate production based on the number of new steps.
    /// Called by PlayerData whenever it detects new steps.
    /// </summary>
    public void AccumulateProduction(int stepDelta)
    {
        // The building produces some resource for each step.
        float produced = stepDelta * resourcesPerStep;
        uncollectedResources += produced;
    }

    /// <summary>
    /// Player collects resources from the building into PlayerData.
    /// </summary>
    public void CollectResources()
    {
        // Example: convert the building's uncollected resources into copper
        long copperToAdd = Mathf.RoundToInt(uncollectedResources);
        PlayerData.Instance.totalCopper += copperToAdd;

        // Reset the building's uncollected storage
        uncollectedResources = 0;

        Debug.Log($"{gameObject.name} gave {copperToAdd} copper to the player!");
    }

    /// <summary>
    /// Whether the building should be producing right now.
    /// (You can base this on whether it’s locked or not, for example.)
    /// </summary>
    public bool IsProducing()
    {
        // For now, we’ll just return true.
        return true;
    }
        //Currently unused, basic implementation for testing
        public void UpgradeBuilding()
    {
        resourcesPerStep += 0.2f;
        Debug.Log("Building upgraded! New production: " + resourcesPerStep);
    }

}
