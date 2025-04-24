using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles clicks on each equipment slot button and delegates
/// equip / unequip requests to EquipmentManager.
/// </summary>
public class EquipSlotContainer : MonoBehaviour
{
    [Header("References")]
    public EquipmentManager equipmentManager;

    [Header("Slot Buttons")]
    public Button helmSlotButton;
    public Button chestSlotButton;
    public Button weaponSlotButton;
    public Button footSlotButton;

    //────────────────────────────────────────────────────────────────
    private void Start()
    {
        // Verify button references
        if (!helmSlotButton || !chestSlotButton || !weaponSlotButton || !footSlotButton)
        {
            Debug.LogError("One or more slot buttons are not assigned.");
            return;
        }

        helmSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Helm));
        chestSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Chest));
        weaponSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Hand));
        footSlotButton.onClick.AddListener(() => OnEquipSlotClicked(EquipmentType.Foot));

        Debug.Log("Equip-slot button listeners assigned.");
    }

    //────────────────────────────────────────────────────────────────
    private void OnEquipSlotClicked(EquipmentType slotType)
    {
        Debug.Log($"Equip slot clicked: {slotType}");

        CharacterData cd = GameManager.Instance.currentCompanionData;
        if (cd == null)
        {
            Debug.LogWarning("No CharacterData selected – can’t modify equipment.");
            return;
        }

        // If something is already equipped → Unequip
        if (cd.GetEquipped(slotType) != null)
        {
            Debug.Log($"Unequipping item from {slotType}");
            equipmentManager.Unequip(slotType);
        }
        else
        {
            // Try to fetch a matching item from the player’s inventory
            Equipment equip = GameManager.Instance.GetItemForSlot(slotType);
            if (equip != null)
            {
                Debug.Log($"Equipping {equip.itemName}");
                equipmentManager.Equip(equip);
            }
            else
            {
                Debug.LogWarning($"No item available for slot {slotType}");
            }
        }
    }
}
