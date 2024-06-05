using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class EquipSlotContainer : MonoBehaviour
{
    public EquipmentManager equipmentManager;

    public Button helmSlotButton;
    public Button chestSlotButton;
    public Button weaponSlotButton;
    public Button footSlotButton;

    private void Start()
    {
        // Ensure the buttons are assigned
        if (helmSlotButton == null || chestSlotButton == null || weaponSlotButton == null || footSlotButton == null)
        {
            Debug.LogError("One or more slot buttons are not assigned.");
            return;
        }

        helmSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Helm));
        chestSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Chest));
        weaponSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Hand));
        footSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Foot));

        Debug.Log("Button click listeners assigned.");
    }

    private void OnEquipSlotClicked(EquipmentType slotType)
    {
        Debug.Log("Equip slot clicked: " + slotType);

        var companion = GameManager.Instance.currentCompanion;
        if (companion == null)
        {
            Debug.LogWarning("No companion assigned. Cannot equip/unequip item.");
            return;
        }

        if (companion.equippedItems.ContainsKey(slotType) && companion.equippedItems[slotType] != null)
        {
            // Unequip the item if the slot is already occupied
            Debug.Log("Unequipping item from slot: " + slotType);
            equipmentManager.Unequip(slotType);
        }
        else
        {
            // Equip a new item if the slot is empty
            Equipment equipment = GameManager.Instance.GetItemForSlot(slotType);
            if (equipment != null)
            {
                Debug.Log("Equipping item: " + equipment.itemName);
                equipmentManager.Equip(equipment);
            }
            else
            {
                Debug.LogWarning("No item available for slot type: " + slotType);
            }
        }
    }
}
