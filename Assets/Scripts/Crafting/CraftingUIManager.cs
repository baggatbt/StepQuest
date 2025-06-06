using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dynamically shows only those recipes whose requiredCraftingLevel
/// is <= the player's current crafting level. Whenever the craftingLevel
/// changes, just call RefreshRecipeList() again to update the buttons.
/// </summary>
public class CraftingUIManager : MonoBehaviour
{
    [Header("References (drag in the Inspector)")]

    [Tooltip("Your CraftingManager instance (contains allRecipes & GetCraftingLevel).")]
    public CraftingManager craftingManager;

    [Tooltip("Parent transform under which we’ll put each RecipeButton.")]
    public Transform recipeButtonContainer;

    [Tooltip("A prefab with a Button + RecipeButton script on it.")]
    public GameObject recipeButtonPrefab;

    private void Start()
    {
        // Build the UI once at start.
        RefreshRecipeList();
    }
    private void OnEnable()
    {
        // Subscribe to the event
        craftingManager.OnCraftingLevelChanged += RefreshRecipeList;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        craftingManager.OnCraftingLevelChanged -= RefreshRecipeList;
    }


    /// <summary>
    /// Call this whenever craftingManager.craftingLevel changes,
    /// or whenever you want to rebuild the entire set of buttons.
    /// </summary>
    public void RefreshRecipeList()
    {
        // 1) Remove existing buttons
        foreach (Transform child in recipeButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 2) Get current player level
        int playerLevel = craftingManager.GetCraftingLevel();

        // 3) Loop through all recipes, instantiate only unlocked ones
        foreach (var recipe in craftingManager.allRecipes)
        {
            if (recipe.requiredCraftingLevel <= playerLevel)
            {
                // Instantiate a new button under the container
                GameObject go = Instantiate(recipeButtonPrefab, recipeButtonContainer);
                // Set its RecipeButton fields:
                RecipeButton rb = go.GetComponent<RecipeButton>();
                if (rb != null)
                {
                    rb.craftingManager = craftingManager;
                    rb.recipe = recipe;
                }
                else
                {
                    Debug.LogError("recipeButtonPrefab is missing a RecipeButton component!");
                }

                // set the button’s visual text/icon:
                 TextMeshProUGUI nameText = go.GetComponentInChildren<TextMeshProUGUI>();
                 if (nameText != null)
                     nameText.text = recipe.outputItem.itemName;
            }
        }
    }
}
