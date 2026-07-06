using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private Transform inventoryUIRoot;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Canvas Group Optional")]
    [SerializeField] private CanvasGroup inventoryCanvasGroup;

    [Header("Optional Equip Support")]
    [SerializeField] private EquipmentManager equipmentManager;

    [Header("Item Detail Panel")]
    [SerializeField] private EquipmentDetailPanelUI equipmentDetailPanel;

    private EquipmentType? pendingEquipSlot = null;

    private void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.inventory = this;

        if (inventoryCanvasGroup == null && inventoryPanel != null)
            inventoryCanvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Invoke(nameof(DelayedRefresh), 0.05f);
    }

    private void OnEnable()
    {
        Invoke(nameof(DelayedRefresh), 0.05f);
    }

    private void DelayedRefresh()
    {
        UpdateInventoryUI();
    }

    public void OpenInventory()
    {
        pendingEquipSlot = null;
        ShowInventory();
        UpdateInventoryUI();
    }

    public void OpenInventoryForEquip(EquipmentType slot)
    {
        pendingEquipSlot = slot;
        ShowInventory();
        UpdateInventoryUI();
    }

    public void CloseInventory()
    {
        pendingEquipSlot = null;

        if (equipmentDetailPanel != null)
            equipmentDetailPanel.Hide();

        HideInventory();
    }

    public void HandleItemClicked(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Inventory] Clicked null item.");
            return;
        }

        // Equip-slot mode:
        // Used when OpenInventoryForEquip(slot) is active.
        if (pendingEquipSlot.HasValue)
        {
            if (item is Equipment equipmentToEquip)
            {
                if (equipmentManager == null)
                {
                    Debug.LogWarning("[Inventory] EquipmentManager is not assigned.");
                    return;
                }

                if (equipmentToEquip.equipmentType != pendingEquipSlot.Value)
                {
                    Debug.Log($"[Inventory] {equipmentToEquip.itemName} cannot go in {pendingEquipSlot.Value} slot.");
                    return;
                }

                equipmentManager.Equip(equipmentToEquip);
                pendingEquipSlot = null;

                if (equipmentDetailPanel != null)
                    equipmentDetailPanel.Hide();

                HideInventory();
                UpdateInventoryUI();
                return;
            }

            Debug.Log("[Inventory] Clicked non-equipment item while trying to equip.");
            return;
        }

        // Normal inventory mode:
        // Equipment opens the new equipment detail panel.
        if (item is Equipment equipment)
        {
            if (equipmentDetailPanel == null)
            {
                Debug.LogWarning("[Inventory] EquipmentDetailPanelUI is not assigned.");
                return;
            }

            equipmentDetailPanel.Show(equipment);
            return;
        }

        // Non-equipment items can still use your old item detail panel if you want.
        MainMenuUIManager uiManager = FindObjectOfType<MainMenuUIManager>();

        if (uiManager != null)
        {
            uiManager.ShowItemDescription(item);
        }
        else
        {
            Debug.Log("[Inventory] Clicked non-equipment item: " + item.itemName);
        }
    }

    private void ShowInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.alpha = 1f;
            inventoryCanvasGroup.interactable = true;
            inventoryCanvasGroup.blocksRaycasts = true;
        }
        else if (inventoryUIRoot != null)
        {
            inventoryUIRoot.gameObject.SetActive(true);
        }
    }

    private void HideInventory()
    {
        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.alpha = 0f;
            inventoryCanvasGroup.interactable = false;
            inventoryCanvasGroup.blocksRaycasts = false;
        }
        else if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        else if (inventoryUIRoot != null)
        {
            inventoryUIRoot.gameObject.SetActive(false);
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUIRoot == null)
        {
            Debug.LogWarning("[Inventory] inventoryUIRoot is not assigned.");
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogWarning("[Inventory] slotPrefab is not assigned.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[Inventory] GameManager.Instance is null.");
            return;
        }

        foreach (Transform child in inventoryUIRoot)
        {
            Destroy(child.gameObject);
        }

        int totalSlots = GameManager.Instance.maxInventorySlots;
        int itemCount = GameManager.Instance.itemList != null ? GameManager.Instance.itemList.Count : 0;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryUIRoot);
            slotObj.SetActive(true);

            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI == null)
            {
                Debug.LogError($"Inventory slot prefab missing InventorySlotUI on {slotObj.name}");
                continue;
            }

            slotUI.Setup(this, i);

            if (i >= itemCount)
            {
                slotUI.ClearVisual();
                continue;
            }

            Item item = GameManager.Instance.itemList[i];

            if (item == null)
            {
                slotUI.ClearVisual();
                continue;
            }

            slotUI.Bind(item, item.quantity, rootCanvas);
        }

        Debug.Log($"[Inventory] UI refreshed. Items shown: {itemCount}");
    }
}