using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI;               // the root panel GameObject
    public CanvasGroup inventoryCanvasGroup;     // ASSIGN in Inspector (on the same root)
    public GameObject slotPrefab;
    public GameObject unequipButtonPrefab;
    public EquipmentManager equipmentManager;

    private EquipmentType pendingEquipSlot;

    private void Awake()
    {
        // Optional: ensure GameManager can find us
        if (GameManager.Instance) GameManager.Instance.inventory = this;
    }

    private void Start()
    {
        GameManager.Instance.LoadInventory();
        // Optional: ensure hidden state at start
        HideInventory();
    }

    // --- PUBLIC API ---

    public void OpenInventoryForEquip(EquipmentType slot)
    {
        pendingEquipSlot = slot;
        ShowInventory();         // ← ensure CG is visible and clickable
        UpdateInventoryUI();
    }

    public void CloseInventory()
    {
        HideInventory();
    }

    // --- INTERNAL ---

    private void ShowInventory()
    {
        if (inventoryUI != null) inventoryUI.SetActive(true);

        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.alpha = 1f;
            inventoryCanvasGroup.interactable = true;
            inventoryCanvasGroup.blocksRaycasts = true;
        }
    }

    private void HideInventory()
    {
        if (inventoryUI != null) inventoryUI.SetActive(true); // keep active so layout stays; CG will gate input

        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.alpha = 0f;
            inventoryCanvasGroup.interactable = false;
            inventoryCanvasGroup.blocksRaycasts = false;
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUI == null) return;

        foreach (Transform child in inventoryUI.transform)
            Destroy(child.gameObject);

        // Unequip button
        Equipment equippedItem = GameManager.Instance.currentCompanionData?.GetEquipped(pendingEquipSlot);
        if (equippedItem != null && unequipButtonPrefab != null)
        {
            GameObject unequipSlot = Instantiate(unequipButtonPrefab, inventoryUI.transform);
            Button unequipBtn = unequipSlot.GetComponent<Button>();
            TextMeshProUGUI label = unequipSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = $"Unequip {equippedItem.itemName}";
            if (unequipBtn)
            {
                unequipBtn.onClick.RemoveAllListeners();
                unequipBtn.onClick.AddListener(() =>
                {
                    equipmentManager.Unequip(pendingEquipSlot);
                    HideInventory();
                });
            }
        }

        // Render inventory slots...
        var items = GameManager.Instance.itemList;
        int totalSlots = GameManager.Instance.maxInventorySlots;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            slot.SetActive(true);

            Image itemImage = slot.transform.Find("ItemContainer")?.GetComponent<Image>();
            TextMeshProUGUI itemCountText = slot.transform.Find("ItemCountText")?.GetComponent<TextMeshProUGUI>();
            Button equipButton = slot.transform.Find("UseButton")?.GetComponent<Button>();

            if (i < items.Count)
            {
                Item item = items[i];

                if (itemImage)
                {
                    itemImage.enabled = true;
                    itemImage.sprite = item.itemIcon;
                }
                if (itemCountText) itemCountText.text = item.quantity.ToString();

                if (equipButton)
                {
                    equipButton.onClick.RemoveAllListeners();

                    if (item is Equipment eq)
                    {
                        bool isCorrectSlot = eq.equipmentType == pendingEquipSlot;
                        equipButton.interactable = isCorrectSlot;

                        if (isCorrectSlot)
                        {
                            Equipment localEq = eq;
                            equipButton.onClick.AddListener(() =>
                            {
                                equipmentManager.Equip(localEq);
                                HideInventory();
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
                if (itemImage) { itemImage.enabled = false; itemImage.sprite = null; }
                if (itemCountText) itemCountText.text = "";
                if (equipButton) { equipButton.interactable = false; equipButton.onClick.RemoveAllListeners(); }
            }
        }
    }
}
