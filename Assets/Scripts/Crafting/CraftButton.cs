using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour
{
    public CraftingManager craftingManager; 
    public CraftableItem craftableItem;  // The item to craft when this button is clicked

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCraftClicked);
    }

    private void OnDestroy()
    {
        // Good practice to remove the listener to avoid memory leaks
        button.onClick.RemoveListener(OnCraftClicked);
    }

    // This method is called when the button is clicked
    private void OnCraftClicked()
    {
        // Pass the selected item to your CraftingManager
        if (craftingManager != null && craftableItem != null)
        {
            craftingManager.OnCraftButtonClicked(craftableItem);
        }
        else
        {
            Debug.LogWarning("CraftingManager or CraftableItem is not set on this button!");
        }
    }
}
