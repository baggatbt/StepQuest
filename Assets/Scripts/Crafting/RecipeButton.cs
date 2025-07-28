using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RecipeButton : MonoBehaviour
{
    public CraftingManager craftingManager;
    public Recipe recipe;

    [Header("UI References")]
    public Image outputItemIcon;
    public TextMeshProUGUI outputQuantityText;
    public TextMeshProUGUI titleText;
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
        UpdateDisplay(); // Refresh UI after crafting
    }

    private void UpdateDisplay()
    {
        if (recipe == null || recipe.outputItem == null) return;

        if (titleText != null)
            titleText.text = recipe.outputItem.itemName;

        if (outputItemIcon != null && recipe.outputItem.itemIcon != null)
            outputItemIcon.sprite = recipe.outputItem.itemIcon;

        int craftableCount = GetCraftableCount(recipe);
        if (outputQuantityText != null)
            outputQuantityText.text = $"x{craftableCount * recipe.outputQuantity}";

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
            if (req.material == null) continue;

            GameObject go = Instantiate(materialRequirementPrefab, requirementsContainer);
            var img = go.transform.Find("Icon")?.GetComponent<Image>();
            var txt = go.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (img != null) img.sprite = req.material.itemIcon;
            if (txt != null) txt.text = $"x{req.quantity}";
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
                return 0;
            }
        }

        return minCount;
    }
}
