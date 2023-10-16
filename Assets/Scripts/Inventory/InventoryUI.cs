using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;
    public Image[] itemSlots;  // Drag item slot images from the scene here

    public void UpdateUI()
    {
        // Clear all item slot visuals
        foreach (Image image in itemSlots)
        {
            image.sprite = null;
            image.enabled = false;  // hide image
        }

        // Loop through inventory items and display them
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            itemSlots[i].sprite = playerInventory.items[i].itemIcon;
            itemSlots[i].enabled = true;  // show image
        }
    }
}
