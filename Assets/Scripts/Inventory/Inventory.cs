using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI; // Drag your InventoryUI GameObject here
    public GameObject slotPrefab; // Drag your Slot Prefab here
    public int maxInventorySlots = 16; // Maximum number of slots
    public Item testItem;

    private List<Item> items = new List<Item>();
    private Dictionary<int, int> materialCounts = new Dictionary<int, int>(); // Track material quantities
    private List<GameObject> itemSlots = new List<GameObject>();

    private void Start()
    {
        // Initialize empty slots
        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            slot.SetActive(true);
            itemSlots.Add(slot);
        }
    }

    public bool AddItem(Item item)
    {
        if (items.Count >= maxInventorySlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        items.Add(item);
        // If the item is a material, update the material count
        if (item is MaterialItem materialItem)
        {
            if (materialCounts.ContainsKey(item.itemID))
            {
                materialCounts[item.itemID] += 1; // Assuming each material item represents a single unit
            }
            else
            {
                materialCounts[item.itemID] = 1;
            }
        }
        UpdateInventoryUI();
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            // If removed item is a material, decrease its count
            if (item is MaterialItem && materialCounts.ContainsKey(item.itemID))
            {
                materialCounts[item.itemID] -= 1;
                if (materialCounts[item.itemID] <= 0)
                {
                    materialCounts.Remove(item.itemID);
                }
            }
            UpdateInventoryUI();
        }
    }

    private void UpdateInventoryUI()
    {
        // Then enable and update necessary slots
        for (int i = 0; i < items.Count; i++)
        {
            itemSlots[i].SetActive(true);

            // Find the ItemContainer and then the Image inside it
            Image itemImage = itemSlots[i].transform.Find("ItemContainer").GetComponentInChildren<Image>();
            if (itemImage != null)
                itemImage.sprite = items[i].itemIcon;
        }
    }

    public bool TryCraftItem(CraftableItem itemToCraft)
    {
        // Check if the player has all necessary materials in sufficient quantities.
        foreach (var requirement in itemToCraft.materialRequirements)
        {
            if (!materialCounts.ContainsKey(requirement.material.itemID) || materialCounts[requirement.material.itemID] < requirement.quantity)
            {
                Debug.Log("Not enough materials to craft " + itemToCraft.itemName);
                return false;
            }
        }

        // Consume the materials.
        foreach (var requirement in itemToCraft.materialRequirements)
        {
            materialCounts[requirement.material.itemID] -= requirement.quantity;
            // Consider removing the material from items list if its count goes to zero
        }

        // Optionally, add the crafted item to the player's inventory.
        Debug.Log("Crafted " + itemToCraft.itemName);
        AddItem(itemToCraft); // This line adds the crafted item to the inventory
        return true;
    }

    public void AddTestItem()
    {
        AddItem(testItem);
    }
}
