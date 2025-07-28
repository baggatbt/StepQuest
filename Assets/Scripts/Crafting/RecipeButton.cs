using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RecipeButton : MonoBehaviour
{
    [Tooltip("Drag your CraftingManager here")]
    public CraftingManager craftingManager;

    [Tooltip("Drag the Recipe asset you want this button to trigger")]
    public Recipe recipe;

    [Header("UI References")]
    public Image outputItemIcon; // <-- Drag your icon Image here
    public TextMeshProUGUI quantityText; // <-- Shows how many can be crafted
    public Transform requirementsContainer;
    public GameObject materialRequirementPrefab;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
            Debug.LogError("RecipeButton requires a Button component!");

        _button.onClick.AddListener(OnClicked);

        UpdateDisplay();
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
            Debug.LogWarning("Missing CraftingManager or Recipe on RecipeButton.");
            return;
        }
        craftingManager.StartCrafting(recipe);
        UpdateDisplay(); // Update UI after crafting
    }

    private void UpdateDisplay()
{
    if (recipe == null)
    {
        Debug.LogError("[RecipeButton] No recipe assigned.");
        return;
    }

    if (recipe.outputItem == null)
    {
        Debug.LogWarning($"[RecipeButton] Recipe {recipe.name} has no outputItem assigned.");
        return;
    }

    if (outputItemIcon == null)
    {
        Debug.LogError("[RecipeButton] OutputItemIcon is not assigned.");
    }
    else
    {
        if (recipe.outputItem.itemIcon == null)
            Debug.LogWarning($"[RecipeButton] Item {recipe.outputItem.itemName} has no icon assigned.");
        else
            outputItemIcon.sprite = recipe.outputItem.itemIcon;
    }

    int craftableCount = GetCraftableCount(recipe);
    if (quantityText != null)
    {
        quantityText.text = $"x{craftableCount}";
    }

    if (requirementsContainer == null || materialRequirementPrefab == null)
    {
        Debug.LogError("[RecipeButton] RequirementsContainer or MaterialRequirementPrefab not assigned.");
        return;
    }

    foreach (Transform child in requirementsContainer)
    {
        Destroy(child.gameObject);
    }

    foreach (var req in recipe.materialRequirements)
    {
        if (req.material == null)
        {
            Debug.LogWarning($"[RecipeButton] One of the recipe's materials is null in {recipe.name}.");
            continue;
        }

        var go = Instantiate(materialRequirementPrefab, requirementsContainer);
        var img = go.GetComponentInChildren<Image>();
        var txt = go.GetComponentInChildren<TextMeshProUGUI>();
        if (img != null) img.sprite = req.material.itemIcon;
        if (txt != null) txt.text = req.quantity.ToString();
    }
}


    private int GetCraftableCount(Recipe recipe)
    {
        int minCount = int.MaxValue;

        foreach (var req in recipe.materialRequirements)
        {
            var foundItem = GameManager.Instance.itemList.Find(i => i.itemID == req.material.itemID);
            if (foundItem != null)
            {
                int possible = foundItem.quantity / req.quantity;
                if (possible < minCount)
                    minCount = possible;
            }
            else
            {
                return 0; // Missing at least one material
            }
        }

        return minCount;
    }
}
