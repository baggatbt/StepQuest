using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles equipping / unequipping through the UI.
/// Talks directly to CharacterData instead of Companion.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    [Header("UI slot images")]
    public Image helmSlotImage;
    public Image chestSlotImage;
    public Image weaponSlotImage;
    public Image footSlotImage;

    // Shortcut to whichever CharacterData the UI is showing
    private CharacterData Current => GameManager.Instance?.currentCompanionData;

    public static event System.Action<CharacterData> OnEquipmentChanged;

    private void Start()
    {
        if (!helmSlotImage) Debug.LogError("HelmSlotImage reference missing.");
        if (!chestSlotImage) Debug.LogError("ChestSlotImage reference missing.");
        if (!weaponSlotImage) Debug.LogError("WeaponSlotImage reference missing.");
        if (!footSlotImage) Debug.LogError("FootSlotImage reference missing.");

        UpdateUI();
    }

    public void Equip(Equipment equipment)
    {
        if (Current == null)
        {
            Debug.LogWarning("No CharacterData selected – can’t equip.");
            return;
        }

        if (equipment == null)
        {
            Debug.LogWarning("Tried to equip null equipment.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null – can’t equip.");
            return;
        }

        Debug.Log($"Equip {equipment.itemName} | ID: {equipment.uniqueInstanceId}");

        // If there is already equipment in this slot, return it to inventory first.
        Equipment currentlyEquipped = Current.GetEquipped(equipment.equipmentType);

        if (currentlyEquipped != null)
        {
            Debug.Log($"Replacing equipped item: {currentlyEquipped.itemName}");
            Current.UnequipItem(equipment.equipmentType);
            GameManager.Instance.AddItem(currentlyEquipped, 1);
        }

        // Equip the selected item.
        Current.EquipItem(equipment);

        // Remove the equipped item from inventory safely.
        bool removed = RemoveEquipmentFromInventory(equipment);

        if (!removed)
        {
            Debug.LogWarning($"[EquipmentManager] Equipped {equipment.itemName}, but could not find it in inventory to remove.");
        }

        Current.SaveData();
        GameManager.Instance.SaveInventory();

        UpdateUI();

        if (GameManager.Instance.inventory != null)
            GameManager.Instance.inventory.UpdateInventoryUI();

        OnEquipmentChanged?.Invoke(Current);
    }

    public void Unequip(EquipmentType slot)
    {
        if (Current == null)
        {
            Debug.LogWarning("No CharacterData selected.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null – can’t unequip.");
            return;
        }

        Equipment toUnequip = Current.GetEquipped(slot);

        if (toUnequip == null)
            return;

        Debug.Log($"Unequip {slot}: {toUnequip.itemName}");

        Current.UnequipItem(slot);
        GameManager.Instance.AddItem(toUnequip, 1);

        Current.SaveData();
        GameManager.Instance.SaveInventory();

        UpdateUI();

        if (GameManager.Instance.inventory != null)
            GameManager.Instance.inventory.UpdateInventoryUI();

        OnEquipmentChanged?.Invoke(Current);
    }

    private bool RemoveEquipmentFromInventory(Equipment equipment)
    {
        if (equipment == null)
            return false;

        if (GameManager.Instance == null || GameManager.Instance.itemList == null)
            return false;

        // First try direct object reference.
        if (GameManager.Instance.itemList.Remove(equipment))
        {
            Debug.Log($"[EquipmentManager] Removed by direct reference: {equipment.itemName}");
            return true;
        }

        // Then try unique instance ID. This is the important part for crafted/loaded equipment.
        if (!string.IsNullOrEmpty(equipment.uniqueInstanceId))
        {
            for (int i = GameManager.Instance.itemList.Count - 1; i >= 0; i--)
            {
                Item item = GameManager.Instance.itemList[i];

                if (item is Equipment inventoryEquipment)
                {
                    if (inventoryEquipment.uniqueInstanceId == equipment.uniqueInstanceId)
                    {
                        GameManager.Instance.itemList.RemoveAt(i);
                        Debug.Log($"[EquipmentManager] Removed by uniqueInstanceId: {equipment.itemName}");
                        return true;
                    }
                }
            }
        }

        // Last-resort fallback for old test equipment that might not have unique IDs.
        for (int i = GameManager.Instance.itemList.Count - 1; i >= 0; i--)
        {
            Item item = GameManager.Instance.itemList[i];

            if (item is Equipment inventoryEquipment)
            {
                bool sameItemId = inventoryEquipment.itemID == equipment.itemID;
                bool sameSlot = inventoryEquipment.equipmentType == equipment.equipmentType;
                bool sameName = inventoryEquipment.itemName == equipment.itemName;

                if (sameItemId && sameSlot && sameName)
                {
                    GameManager.Instance.itemList.RemoveAt(i);
                    Debug.LogWarning($"[EquipmentManager] Removed by fallback item match: {equipment.itemName}");
                    return true;
                }
            }
        }

        return false;
    }

    private void UpdateUI()
    {
        if (Current == null)
        {
            ClearAllSlots();
            return;
        }

        SetSlotImage(helmSlotImage, EquipmentType.Helm);
        SetSlotImage(chestSlotImage, EquipmentType.Chest);
        SetSlotImage(weaponSlotImage, EquipmentType.Hand);
        SetSlotImage(footSlotImage, EquipmentType.Foot);
    }

    private void SetSlotImage(Image img, EquipmentType slot)
    {
        if (img == null)
            return;

        Equipment eq = Current.GetEquipped(slot);

        if (eq != null)
        {
            img.sprite = eq.itemIcon;
            img.color = Color.white;
        }
        else
        {
            img.sprite = null;
            img.color = Color.clear;
        }
    }

    private void ClearAllSlots()
    {
        foreach (var img in new[] { helmSlotImage, chestSlotImage, weaponSlotImage, footSlotImage })
        {
            if (img == null)
                continue;

            img.sprite = null;
            img.color = Color.clear;
        }
    }
}