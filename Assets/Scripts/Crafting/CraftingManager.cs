using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    private Dictionary<string, CraftableItem> activeCrafts = new Dictionary<string, CraftableItem>();

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (TimerManager.Instance != null)
            TimerManager.Instance.OnTimerCompleted += HandleTimerCompleted;
        else
            Debug.LogError("TimerManager.Instance is null. Crafting timers will not work.");
    }

    private void OnDestroy()
    {
        if (TimerManager.Instance != null)
            TimerManager.Instance.OnTimerCompleted -= HandleTimerCompleted;
    }

    public void OnCraftButtonClicked(CraftableItem craftableItem)
    {
        if (craftableItem == null)
        {
            Debug.LogWarning("Tried to craft a null CraftableItem.");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogError("CraftingManager has no GameManager reference.");
            return;
        }

        if (!HasAllMaterials(craftableItem))
        {
            Debug.LogWarning("Not enough materials to craft: " + craftableItem.itemName);
            return;
        }

        ConsumeMaterials(craftableItem);

        string timerId = GenerateCraftTimerId(craftableItem);
        float craftDuration = Mathf.Max(0.1f, craftableItem.stepCostToProduce);

        TimerManager.Instance.SetTimer(timerId, craftDuration);
        activeCrafts[timerId] = craftableItem;

        Debug.Log($"Started crafting {craftableItem.itemName} for {craftDuration} seconds.");
    }

    private void HandleTimerCompleted(string timerId)
    {
        if (!activeCrafts.ContainsKey(timerId))
            return;

        CraftableItem craftableItem = activeCrafts[timerId];
        activeCrafts.Remove(timerId);

        CompleteCraft(craftableItem);
    }

    private void CompleteCraft(CraftableItem craftableItem)
    {
        if (craftableItem.CraftsEquipment())
        {
            Equipment equipmentTemplate = craftableItem.GetEquipmentCraftResult();

            if (equipmentTemplate == null)
            {
                Debug.LogError("CraftableItem was set to craft equipment, but no equipment result was assigned.");
                return;
            }

            Equipment rolledEquipment = Instantiate(equipmentTemplate);
            rolledEquipment.RollNewStats();

            gameManager.AddItem(rolledEquipment);

            Debug.Log($"Crafting complete: {rolledEquipment.itemName}\n{rolledEquipment.GetStatDescription()}");
            return;
        }

        Item normalResult = craftableItem.GetNormalCraftResult();

        if (normalResult == null)
        {
            Debug.LogError($"CraftableItem {craftableItem.itemName} has no craft result assigned.");
            return;
        }

        gameManager.AddItem(normalResult);

        Debug.Log($"Crafting complete. {normalResult.itemName} added to inventory.");
    }

    private bool HasAllMaterials(CraftableItem craftableItem)
    {
        if (craftableItem.materialRequirements == null || craftableItem.materialRequirements.Count == 0)
        {
            Debug.LogWarning($"Recipe {craftableItem.itemName} has no material requirements assigned.");
            return false;
        }

        Debug.Log($"Checking materials for recipe: {craftableItem.itemName}");

        foreach (Item item in gameManager.itemList)
        {
            if (item == null) continue;

            Debug.Log($"Inventory has: {item.itemName} | ID: {item.itemID} | Qty: {item.quantity}");
        }

        foreach (MaterialRequirement requirement in craftableItem.materialRequirements)
        {
            if (requirement.material == null)
            {
                Debug.LogWarning($"Recipe {craftableItem.itemName} has a null material requirement.");
                return false;
            }

            int requiredItemID = requirement.material.itemID;
            int requiredQuantity = requirement.quantity;
            int foundQuantity = GetItemQuantityInItemList(requiredItemID);

            Debug.Log($"Recipe needs: {requirement.material.itemName} | ID: {requiredItemID} | Qty: {requiredQuantity} | Found: {foundQuantity}");

            if (foundQuantity < requiredQuantity)
            {
                Debug.LogWarning(
                    $"Missing material: {requirement.material.itemName} | Needed ID: {requiredItemID} | Needed Qty: {requiredQuantity} | Found Qty: {foundQuantity}"
                );

                return false;
            }
        }

        return true;
    }

    private void ConsumeMaterials(CraftableItem craftableItem)
    {
        foreach (MaterialRequirement requirement in craftableItem.materialRequirements)
        {
            if (requirement.material == null)
            {
                Debug.LogWarning($"Recipe {craftableItem.itemName} has a null material requirement.");
                continue;
            }

            RemoveItemFromItemList(requirement.material.itemID, requirement.quantity);

            Debug.Log($"Consumed {requirement.quantity}x {requirement.material.itemName}");
        }

        gameManager.SaveInventory();

        if (gameManager.inventory != null)
            gameManager.inventory.UpdateInventoryUI();
    }

    private int GetItemQuantityInItemList(int itemID)
    {
        int total = 0;

        foreach (Item item in gameManager.itemList)
        {
            if (item == null) continue;

            if (item.itemID == itemID)
                total += item.quantity;
        }

        return total;
    }

    private void RemoveItemFromItemList(int itemID, int quantityToRemove)
    {
        int remainingToRemove = quantityToRemove;

        for (int i = gameManager.itemList.Count - 1; i >= 0; i--)
        {
            Item item = gameManager.itemList[i];

            if (item == null) continue;
            if (item.itemID != itemID) continue;

            if (item.quantity > remainingToRemove)
            {
                item.quantity -= remainingToRemove;
                remainingToRemove = 0;
                break;
            }
            else
            {
                remainingToRemove -= item.quantity;
                gameManager.itemList.RemoveAt(i);
            }

            if (remainingToRemove <= 0)
                break;
        }

        if (remainingToRemove > 0)
        {
            Debug.LogWarning($"Tried to remove item ID {itemID}, but was short by {remainingToRemove}.");
        }
    }

    private string GenerateCraftTimerId(CraftableItem item)
    {
        return $"Craft_{item.itemID}_{Time.realtimeSinceStartup}_{Random.Range(0, 999999)}";
    }
}