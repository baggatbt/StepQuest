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
        UpdateInventoryUI();
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
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

    public void AddTestItem()
    {
        AddItem(testItem);
    }
}
