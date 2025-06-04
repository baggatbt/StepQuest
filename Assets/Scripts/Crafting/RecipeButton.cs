using UnityEngine;
using UnityEngine.UI;  // <— needed for Button

public class RecipeButton : MonoBehaviour
{
    [Tooltip("Drag your CraftingManager here")]
    public CraftingManager craftingManager;

    [Tooltip("Drag the Recipe asset you want this button to trigger")]
    public Recipe recipe;

    private Button _button;

    private void Awake()
    {
        // Cache the Button component (requires UnityEngine.UI)
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("RecipeButton requires a Button component on the same GameObject.");
            return;
        }

        _button.onClick.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        if (craftingManager == null || recipe == null)
        {
            Debug.LogWarning("CraftingManager or Recipe is not assigned on RecipeButton.");
            return;
        }

        craftingManager.StartCrafting(recipe);
    }
}
