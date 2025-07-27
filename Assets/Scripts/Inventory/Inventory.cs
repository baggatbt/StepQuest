using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI;
    public GameObject slotPrefab;
    public GameObject unequipButtonPrefab; // <-- Assign a prefab with a Button + Label in Inspector
    public EquipmentManager equipmentManager;

    private EquipmentType pendingEquipSlot;

    private void Start()
    {
        GameManager.Instance.LoadInventory();
    }

    public void OpenInventoryForEquip(EquipmentType slot)
    {
        pendingEquipSlot = slot;
        inventoryUI.SetActive(true);
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUI == null) return;

        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }

        // ───── Add Unequip Button if item is equipped ─────
        Equipment equippedItem = GameManager.Instance.currentCompanionData?.GetEquipped(pendingEquipSlot);
        if (equippedItem != null && unequipButtonPrefab != null)
        {
            GameObject unequipSlot = Instantiate(unequipButtonPrefab, inventoryUI.transform);
            Button unequipBtn = unequipSlot.GetComponent<Button>();
            TextMeshProUGUI label = unequipSlot.GetComponentInChildren<TextMeshProUGUI>();

            if (label != null)
                label.text = $"Unequip {equippedItem.itemName}";

            if (unequipBtn != null)
            {
                unequipBtn.onClick.RemoveAllListeners();
                unequipBtn.onClick.AddListener(() =>
                {
                    Debug.Log($"Clicked to unequip {equippedItem.itemName} from {pendingEquipSlot}");
                    equipmentManager.Unequip(pendingEquipSlot);
                    inventoryUI.SetActive(false);
                });
            }
        }

        // ───── Render Inventory Slots ─────
        var items = GameManager.Instance.itemList;
        int totalSlots = GameManager.Instance.maxInventorySlots;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            slot.SetActive(true);

            Image itemImage = slot.transform.Find("ItemContainer")?.GetComponent<Image>();
            TextMeshProUGUI itemCountText = slot.transform.Find("ItemCountText")?.GetComponent<TextMeshProUGUI>();
            Button equipButton = slot.GetComponent<Button>();

            if (i < items.Count)
            {
                Item item = items[i];

                if (itemImage != null)
                {
                    itemImage.enabled = true;
                    itemImage.sprite = item.itemIcon;
                }

                if (itemCountText != null)
                {
                    itemCountText.text = item.quantity.ToString();
                }

                if (equipButton != null)
                {
                    equipButton.onClick.RemoveAllListeners();

                    if (item is Equipment equipment)
                    {
                        bool isCorrectSlot = equipment.equipmentType == pendingEquipSlot;
                        equipButton.interactable = isCorrectSlot;

                        if (isCorrectSlot)
                        {
                            equipButton.onClick.AddListener(() =>
                            {
                                Debug.Log($"Clicked to equip {equipment.itemName} into {pendingEquipSlot} slot");
                                equipmentManager.Equip(equipment);
                                inventoryUI.SetActive(false);
                            });
                        }
                    }
                    else
                    {
                        equipButton.interactable = false;
                    }
                }
            }
            else
            {
                if (itemImage != null)
                {
                    itemImage.enabled = false;
                    itemImage.sprite = null;
                }

                if (itemCountText != null)
                {
                    itemCountText.text = "";
                }

                if (equipButton != null)
                {
                    equipButton.interactable = false;
                    equipButton.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
