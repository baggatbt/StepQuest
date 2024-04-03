using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI; // Drag your InventoryUI GameObject here
    public GameObject slotPrefab; // Drag your Slot Prefab here
    public int maxInventorySlots = 16; // Maximum number of slots
    public Item testItem;
    private string SavePath => $"{Application.persistentDataPath}/inventory.json";


    private List<Item> items = new List<Item>();
    private Dictionary<int, int> materialCounts = new Dictionary<int, int>(); // Track material quantities
    private List<GameObject> itemSlots = new List<GameObject>();


    private void Awake() {
        LoadInventory();
    }

    public void SaveInventory() {
        var json = JsonUtility.ToJson(new ItemContainer { Items = items }, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Inventory saved to {SavePath}");
    }

    public void LoadInventory() {
        if (File.Exists(SavePath)) {
            var json = File.ReadAllText(SavePath);
            var itemContainer = JsonUtility.FromJson<ItemContainer>(json);
            items = itemContainer.Items;
            UpdateInventoryUI();
            Debug.Log("Inventory loaded.");
        } else {
            Debug.Log("No inventory save found.");
        }
    }

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
        SaveInventory();
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
            SaveInventory();
        }
    }

    private void UpdateInventoryUI() {
    // First, ensure the correct number of slots are available
    while (itemSlots.Count < maxInventorySlots) {
        GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
        slot.SetActive(true);
        itemSlots.Add(slot);
    }

    // Update slots based on inventory items or clear if no item is assigned
    for (int i = 0; i < maxInventorySlots; i++) {
        // Find the ItemContainer and then the Image inside it for each slot
        Image itemImage = itemSlots[i].transform.Find("ItemContainer").GetComponentInChildren<Image>();
        if (i < items.Count && itemImage != null) {
            itemSlots[i].SetActive(true); // Make sure the slot is active
            itemImage.sprite = items[i].itemIcon; // Update the sprite to the current item's icon
            itemImage.enabled = true; // Enable the image component to show the icon
        } else if (itemImage != null) {
            itemImage.sprite = null; // Clear the sprite for slots without an item
            itemImage.enabled = false; // Optionally disable the image component if no sprite is assigned
        }
    }
}


    public void AddTestItem()
    {
        AddItem(testItem);
        
    }

    public void DeleteSavedInventory() {
    string path = SavePath; // Assuming SavePath is your file path

    // Check if the file exists before attempting to delete
    if (File.Exists(path)) {
        File.Delete(path);
        Debug.Log("Saved inventory data deleted.");
    } else {
        Debug.Log("No saved inventory data to delete.");
    }
}


    [System.Serializable]
    class ItemContainer 
    {
          public List<Item> Items;
    }
}
