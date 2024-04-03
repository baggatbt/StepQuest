using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI; // Drag your InventoryUI GameObject here
    public GameObject slotPrefab; // Drag your Slot Prefab here
    public int maxInventorySlots;
    public Item testItem;

    private string SavePath => $"{Application.persistentDataPath}/inventory.json";

    private Dictionary<int, int> materialCounts = new Dictionary<int, int>(); // Track material quantities
    private List<GameObject> itemSlots = new List<GameObject>();

    private void Awake() {
        maxInventorySlots = GameManager.Instance.maxInventorySlots;
    }

    public void SaveInventory() {
        // This method might need rethinking if GameManager is handling items
        var json = JsonUtility.ToJson(new ItemContainer { Items = GameManager.Instance.itemList }, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Inventory saved to {SavePath}");
    }

    public void LoadInventory() {
        // This method might need rethinking if GameManager is handling items
        if (File.Exists(SavePath)) {
            var json = File.ReadAllText(SavePath);
            var itemContainer = JsonUtility.FromJson<ItemContainer>(json);
            GameManager.Instance.itemList = itemContainer.Items;
            UpdateInventoryUI();
            Debug.Log("Inventory loaded.");
        } else {
            Debug.Log("No inventory save found.");
        }
    }

    private void Start() {
        // Initialize empty slots
        for (int i = 0; i < maxInventorySlots; i++) {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            slot.SetActive(true);
            itemSlots.Add(slot);
        }
    }

    // This method needs to be adjusted to interact with GameManager's itemList
    public bool AddItem(Item item) {
        if (GameManager.Instance.itemList.Count >= maxInventorySlots) {
            Debug.Log("Inventory is full!");
            return false;
        }

        GameManager.Instance.AddItem(item); // Assuming GameManager has an AddItem method
        UpdateInventoryUI();
        return true;
    }

    // Adjusted to use GameManager's method for removing an item
    public void RemoveItem(Item item) {
        GameManager.Instance.RemoveItem(item); // Assuming GameManager has a RemoveItem method
        UpdateInventoryUI();
    }

    // Updates the inventory UI to reflect the current items
    private void UpdateInventoryUI() {
    Debug.Log($"Updating UI with {GameManager.Instance.itemList.Count} items.");

    foreach (Transform child in inventoryUI.transform) {
        Destroy(child.gameObject);
    }

    foreach (Item item in GameManager.Instance.itemList) {
        Debug.Log($"Adding item to UI: {item.itemName}");
        GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
        slot.SetActive(true);
        Image itemImage = slot.transform.Find("ItemContainer").GetComponentInChildren<Image>();
        if (itemImage != null && item.itemIcon != null) {
            itemImage.sprite = item.itemIcon;
        } else {
            Debug.LogWarning("Missing item icon or incorrect path in prefab.");
        }
    }
}


    

    public void DeleteSavedInventory() {
        string path = SavePath;

        if (File.Exists(path)) {
            File.Delete(path);
            Debug.Log("Saved inventory data deleted.");
        } else {
            Debug.Log("No saved inventory data to delete.");
        }
    }

    [System.Serializable]
    class ItemContainer {
        public List<Item> Items;
    }
}
