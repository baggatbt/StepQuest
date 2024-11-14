using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public Image helmSlotImage;
    public Image chestSlotImage;
    public Image weaponSlotImage;
    public Image footSlotImage;

    private void Start()
    {
        if (helmSlotImage == null) Debug.LogError("HelmSlotImage reference is missing.");
        if (chestSlotImage == null) Debug.LogError("ChestSlotImage reference is missing.");
        if (weaponSlotImage == null) Debug.LogError("WeaponSlotImage reference is missing.");
        if (footSlotImage == null) Debug.LogError("FootSlotImage reference is missing.");
    }

    public void Equip(Equipment equipment)
    {
        var companion = GameManager.Instance.currentCompanion;
        if (companion == null)
        {
            Debug.LogWarning("No companion assigned. Cannot equip item.");
            return;
        }

        Debug.Log("Equip method called with item: " + equipment.itemName);
        companion.EquipItem(equipment);
         GameManager.Instance.itemList.Remove(equipment); // Remove from inventory
        UpdateUI();
        Debug.Log(companion.heroID + " equipped " + equipment.itemName);
    }

    public void Unequip(EquipmentType equipmentType)
    {
        var companion = GameManager.Instance.currentCompanion;
        if (companion == null)
        {
            Debug.LogWarning("No companion assigned. Cannot unequip item.");
            return;
        }

        if (companion.equippedItems.ContainsKey(equipmentType) && companion.equippedItems[equipmentType] != null)
        {
            Equipment unequippedItem = companion.equippedItems[equipmentType];
            Debug.Log("Unequip method called for slot: " + equipmentType);

            companion.UnequipItem(equipmentType);

            // Add the unequipped item back to the inventory
            GameManager.Instance.AddItem(unequippedItem);

            UpdateUI();
            Debug.Log(companion.heroID + " unequipped " + equipmentType);
        }
    }

    private void UpdateUI()
    {
        var companion = GameManager.Instance.currentCompanion;
        Debug.Log("Updating UI");

        if (companion == null)
        {
            // Clear all slot images if no companion is assigned
            helmSlotImage.sprite = null;
            helmSlotImage.color = Color.clear;
            chestSlotImage.sprite = null;
            chestSlotImage.color = Color.clear;
            weaponSlotImage.sprite = null;
            weaponSlotImage.color = Color.clear;
            footSlotImage.sprite = null;
            footSlotImage.color = Color.clear;
            return;
        }

        UpdateSlotImage(helmSlotImage, EquipmentType.Helm);
        UpdateSlotImage(chestSlotImage, EquipmentType.Chest);
        UpdateSlotImage(weaponSlotImage, EquipmentType.Hand);
        UpdateSlotImage(footSlotImage, EquipmentType.Foot);
    }

    private void UpdateSlotImage(Image slotImage, EquipmentType slotType)
    {
        var companion = GameManager.Instance.currentCompanion;
        if (companion.equippedItems.ContainsKey(slotType))
        {
            slotImage.sprite = companion.equippedItems[slotType].itemIcon;
            slotImage.color = Color.white; // Ensure the image is visible
        }
        else
        {
            slotImage.sprite = null;
            slotImage.color = Color.clear; // Hide the image if no item is equipped
        }
    }
}
