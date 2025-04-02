using UnityEngine;
using TMPro;

public class BuildingUI : MonoBehaviour
{
    // Assign this in the Inspector to your TMP_Text component that shows the production value.
    [SerializeField] private TMP_Text productionText;

    // Reference to the building whose production we're displaying.
    // You can either set this dynamically or use a static reference.
    public Building activeBuilding;

    private void OnEnable()
    {
       UpdateProductionDisplay();
    }

    /*Only update on setting building active for now, easier on resources
    private void Update()
    {
        // Update the production display regularly.
        UpdateProductionDisplay();
    }
    */
    private void UpdateProductionDisplay()
    {
        
        
            // Call the building's method to get the production amount (as a string).
            productionText.text = activeBuilding.uncollectedResources.ToString();
        
    }
}
