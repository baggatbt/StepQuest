using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCraftingPanelUI : MonoBehaviour
{
    [Header("Recipe To Test")]
    [SerializeField] private EquipmentCraftingRecipe recipe;

    [Header("UI References")]
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text resultPreviewText;
    [SerializeField] private Button craftButton;

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        if (craftButton != null)
            craftButton.onClick.AddListener(CraftSelectedRecipe);

        Refresh();
    }

    private void OnDestroy()
    {
        if (craftButton != null)
            craftButton.onClick.RemoveListener(CraftSelectedRecipe);
    }

    public void Refresh()
    {
        if (recipe == null)
        {
            SetText(recipeNameText, "No Recipe Selected");
            SetText(costText, "");
            SetText(resultPreviewText, "");
            SetCraftButton(false);
            return;
        }

        SetText(recipeNameText, recipe.recipeName);

        string costs = BuildCostText(recipe);
        SetText(costText, costs);

        string preview = BuildPreviewText(recipe);
        SetText(resultPreviewText, preview);

        bool canCraft = EquipmentCraftingManager.Instance != null &&
                        EquipmentCraftingManager.Instance.CanCraft(recipe);

        SetCraftButton(canCraft);
    }

    private void CraftSelectedRecipe()
    {
        if (recipe == null)
        {
            Debug.LogWarning("[CraftingPanel] No recipe assigned.");
            return;
        }

        if (EquipmentCraftingManager.Instance == null)
        {
            Debug.LogError("[CraftingPanel] No EquipmentCraftingManager found in scene.");
            return;
        }

        bool crafted = EquipmentCraftingManager.Instance.TryCraft(recipe, out Equipment craftedEquipment);

        if (crafted && craftedEquipment != null)
        {
            SetText(resultPreviewText,
                "Crafted!\n\n" +
                craftedEquipment.itemName + "\n" +
                craftedEquipment.GetStatDescription()
            );

            Debug.Log("[CraftingPanel] Crafted equipment:");
            Debug.Log(craftedEquipment.itemName);
            Debug.Log(craftedEquipment.GetStatDescription());
        }

        Refresh();
    }

    private string BuildCostText(EquipmentCraftingRecipe recipe)
    {
        string text = "Costs:\n";

        foreach (EquipmentMaterialCost cost in recipe.materialCosts)
        {
            if (cost == null || cost.item == null)
                continue;

            int owned = GameManager.Instance != null
                ? GameManager.Instance.GetItemCount(cost.item.itemID)
                : 0;

            text += $"{cost.item.itemName}: {owned}/{cost.amount}\n";
        }

        long currentGold = PlayerData.Instance != null ? PlayerData.Instance.totalCopper : 0;
        long currentSteps = PlayerData.Instance != null ? PlayerData.Instance.inGameSteps : 0;

        text += $"Gold: {currentGold}/{recipe.goldCost}\n";
        text += $"Steps: {currentSteps}/{recipe.stepCost}";

        return text;
    }

    private string BuildPreviewText(EquipmentCraftingRecipe recipe)
    {
        if (recipe.resultEquipmentTemplate == null)
            return "No result equipment assigned.";

        Equipment equipment = recipe.resultEquipmentTemplate;

        string text = $"Creates: {equipment.itemName}\n\n";

        text += "Possible guaranteed stats:\n";

        if (equipment.guaranteedStatRanges == null || equipment.guaranteedStatRanges.Count == 0)
        {
            text += "None\n";
        }
        else
        {
            foreach (EquipmentStatRange range in equipment.guaranteedStatRanges)
            {
                text += $"{range.statType}: {range.minValue}-{range.maxValue}\n";
            }
        }

        if (equipment.secretStatRanges != null && equipment.secretStatRanges.Count > 0)
        {
            text += "\nPossible bonus stats:\n";

            foreach (SecretEquipmentStatRange secret in equipment.secretStatRanges)
            {
                text += $"{secret.statType}: {secret.minValue}-{secret.maxValue} ";
                text += $"({secret.chancePercent}% chance)\n";
            }
        }

        return text;
    }

    private void SetCraftButton(bool interactable)
    {
        if (craftButton != null)
            craftButton.interactable = interactable;
    }

    private void SetText(TMP_Text textComponent, string value)
    {
        if (textComponent != null)
            textComponent.text = value;
    }
}