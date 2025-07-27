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

    // NEW: Just open inventory and store selected slot
    Inventory inventory = GameManager.Instance.inventory;
    if (inventory != null)
    {
        inventory.OpenInventoryForEquip(slotType);
    }
}

}
