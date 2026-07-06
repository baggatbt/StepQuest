using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButtonUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button button;

    private Item currentItem;
    private EquipmentDetailPanelUI equipmentDetailPanel;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);
    }

    public void Setup(Item item, EquipmentDetailPanelUI detailPanel)
    {
        currentItem = item;
        equipmentDetailPanel = detailPanel;

        if (currentItem == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (iconImage != null)
        {
            iconImage.sprite = currentItem.itemIcon;
            iconImage.color = currentItem.itemIcon != null ? Color.white : Color.clear;
        }

        if (nameText != null)
            nameText.text = currentItem.itemName;

        if (quantityText != null)
        {
            if (currentItem is Equipment)
                quantityText.text = "";
            else
                quantityText.text = "x" + currentItem.quantity;
        }
    }

    private void OnClicked()
    {
        if (currentItem == null)
            return;

        if (currentItem is Equipment equipment)
        {
            if (equipmentDetailPanel != null)
                equipmentDetailPanel.Show(equipment);
        }
        else
        {
            Debug.Log("[InventoryItemButton] Clicked non-equipment item: " + currentItem.itemName);
        }
    }
}