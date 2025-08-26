using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles equipping / unequipping through the UI.
/// Now talks directly to CharacterData instead of Companion.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    [Header("UI slot images")]
    public Image helmSlotImage;
    public Image chestSlotImage;
    public Image weaponSlotImage;
    public Image footSlotImage;

    // Shortcut to whichever CharacterData the UI is showing
    private CharacterData Current => GameManager.Instance?.currentCompanionData;

    public static event System.Action<CharacterData> OnEquipmentChanged;



    //────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (!helmSlotImage)   Debug.LogError("HelmSlotImage reference missing.");
        if (!chestSlotImage)  Debug.LogError("ChestSlotImage reference missing.");
        if (!weaponSlotImage) Debug.LogError("WeaponSlotImage reference missing.");
        if (!footSlotImage)   Debug.LogError("FootSlotImage reference missing.");

        UpdateUI();
    }

    //────────────────────────────────────────────────────────────────
    #region Public API (called by inventory buttons, etc.)

    public void Equip(Equipment equipment)
    {
        if (Current == null)
        {
            Debug.LogWarning("No CharacterData selected – can’t equip.");
            return;
        }

        Debug.Log($"Equip {equipment.itemName}");
        Current.EquipItem(equipment);                     // NEW: goes straight to CharacterData
        GameManager.Instance.itemList.Remove(equipment); // remove from inventory
        UpdateUI();
        OnEquipmentChanged?.Invoke(Current); //Updates hero stats right away
    }

    public void Unequip(EquipmentType slot)
    {
        if (Current == null) { Debug.LogWarning("No CharacterData selected."); return; }

        Equipment toUnequip = Current.GetEquipped(slot);
        if (toUnequip == null) return;

        Debug.Log($"Unequip {slot}");
        Current.UnequipItem(slot);
        GameManager.Instance.AddItem(toUnequip);          // back to inventory
        UpdateUI();
        OnEquipmentChanged?.Invoke(Current);
    }

    #endregion
    //────────────────────────────────────────────────────────────────
    #region UI helpers

    private void UpdateUI()
    {
        if (Current == null) { ClearAllSlots(); return; }

        SetSlotImage(helmSlotImage,   EquipmentType.Helm);
        SetSlotImage(chestSlotImage,  EquipmentType.Chest);
        SetSlotImage(weaponSlotImage, EquipmentType.Hand);
        SetSlotImage(footSlotImage,   EquipmentType.Foot);
    }

    private void SetSlotImage(Image img, EquipmentType slot)
    {
        Equipment eq = Current.GetEquipped(slot);

        if (eq != null)
        {
            img.sprite = eq.itemIcon;
            img.color  = Color.white;
        }
        else
        {
            img.sprite = null;
            img.color  = Color.clear;
        }
    }

    private void ClearAllSlots()
    {
        foreach (var img in new[] { helmSlotImage, chestSlotImage, weaponSlotImage, footSlotImage })
        {
            img.sprite = null;
            img.color  = Color.clear;
        }
    }

    #endregion
}
