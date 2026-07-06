using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text slotTypeText;
    [SerializeField] private TMP_Text statText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button closeButton;

    [Header("References")]
    [SerializeField] private EquipmentManager equipmentManager;

    private Equipment currentEquipment;

    private void Awake()
    {
        if (equipButton != null)
            equipButton.onClick.AddListener(EquipCurrentItem);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    private void OnDestroy()
    {
        if (equipButton != null)
            equipButton.onClick.RemoveListener(EquipCurrentItem);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Hide);
    }

    public void Show(Equipment equipment)
    {
        currentEquipment = equipment;

        if (currentEquipment == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        currentEquipment = null;
        gameObject.SetActive(false);
    }

    private void Refresh()
    {
        if (currentEquipment == null)
            return;

        if (itemIconImage != null)
        {
            itemIconImage.sprite = currentEquipment.itemIcon;
            itemIconImage.color = currentEquipment.itemIcon != null ? Color.white : Color.clear;
        }

        if (itemNameText != null)
            itemNameText.text = currentEquipment.itemName;

        if (slotTypeText != null)
            slotTypeText.text = "Slot: " + currentEquipment.equipmentType;

        if (statText != null)
            statText.text = currentEquipment.GetStatDescription();

        if (descriptionText != null)
            descriptionText.text = currentEquipment.itemDescription;

        if (equipButton != null)
        {
            bool canEquip = equipmentManager != null &&
                            GameManager.Instance != null &&
                            GameManager.Instance.currentCompanionData != null;

            equipButton.interactable = canEquip;
        }
    }

    private void EquipCurrentItem()
    {
        if (currentEquipment == null)
        {
            Debug.LogWarning("[EquipmentDetailPanel] No equipment selected.");
            return;
        }

        if (equipmentManager == null)
        {
            Debug.LogError("[EquipmentDetailPanel] EquipmentManager reference missing.");
            return;
        }

        equipmentManager.Equip(currentEquipment);

        Hide();
    }
}