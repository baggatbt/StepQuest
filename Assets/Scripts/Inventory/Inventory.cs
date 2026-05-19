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
        // Important:
        // GameManager should already load inventory in its own Start.
        // This small delayed refresh helps avoid script execution order issues.
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
        HideInventory();
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
        // Since your inventory uses CanvasGroup alpha instead of SetActive,
        // we hide it this way.
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

            // Empty slot
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

            // Optional equip mode
            if (pendingEquipSlot.HasValue && equipmentManager != null && item is Equipment eq)
            {
                Button button = slotObj.GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.RemoveAllListeners();

                    if (eq.equipmentType == pendingEquipSlot.Value)
                    {
                        Equipment equipmentToEquip = eq;

                        button.onClick.AddListener(() =>
                        {
                            equipmentManager.Equip(equipmentToEquip);
                            pendingEquipSlot = null;
                            HideInventory();
                            UpdateInventoryUI();
                        });
                    }
                }
            }
        }

        Debug.Log($"[Inventory] UI refreshed. Items shown: {itemCount}");
    }
}