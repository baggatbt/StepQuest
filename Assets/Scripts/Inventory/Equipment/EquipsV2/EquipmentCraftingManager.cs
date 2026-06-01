using System.Collections.Generic;
using UnityEngine;

public class EquipmentCraftingManager : MonoBehaviour
{
    public static EquipmentCraftingManager Instance { get; private set; }

    [Header("Available Recipes")]
    [SerializeField] private List<EquipmentCraftingRecipe> availableRecipes = new List<EquipmentCraftingRecipe>();

    public IReadOnlyList<EquipmentCraftingRecipe> AvailableRecipes => availableRecipes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool CanCraft(EquipmentCraftingRecipe recipe)
    {
        if (recipe == null)
            return false;

        if (recipe.resultEquipmentTemplate == null)
            return false;

        if (GameManager.Instance == null)
            return false;

        if (PlayerData.Instance == null)
            return false;

        foreach (EquipmentMaterialCost cost in recipe.materialCosts)
        {
            if (cost == null || cost.item == null)
                continue;

            if (!GameManager.Instance.HasItem(cost.item, cost.amount))
                return false;
        }

        if (PlayerData.Instance.totalCopper < recipe.goldCost)
            return false;

        if (PlayerData.Instance.inGameSteps < recipe.stepCost)
            return false;

        return true;
    }

    public bool TryCraft(EquipmentCraftingRecipe recipe, out Equipment craftedEquipment)
    {
        craftedEquipment = null;

        if (!CanCraft(recipe))
        {
            Debug.Log("[EquipmentCrafting] Cannot craft. Missing materials, gold, steps, or recipe data.");
            return false;
        }

        foreach (EquipmentMaterialCost cost in recipe.materialCosts)
        {
            if (cost == null || cost.item == null)
                continue;

            bool removed = GameManager.Instance.RemoveItem(cost.item.itemID, cost.amount);

            if (!removed)
            {
                Debug.LogError("[EquipmentCrafting] Failed to remove material: " + cost.item.itemName);
                return false;
            }
        }

        PlayerData.Instance.totalCopper -= recipe.goldCost;
        PlayerData.Instance.inGameSteps -= recipe.stepCost;
        PlayerData.Instance.SavePlayerData();

        craftedEquipment = Instantiate(recipe.resultEquipmentTemplate);

        craftedEquipment.RollNewStats();

        GameManager.Instance.AddItem(craftedEquipment, 1);

        Debug.Log("[EquipmentCrafting] Crafted: " + craftedEquipment.itemName);
        Debug.Log(craftedEquipment.GetStatDescription());

        return true;
    }
}