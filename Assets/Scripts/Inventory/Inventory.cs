using UnityEngine;
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private Transform inventoryUIRoot;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private GameObject inventoryPanel;

    [Header("Optional Equip Support")]
    [SerializeField] private EquipmentManager equipmentManager;

    private EquipmentType? pendingEquipSlot = null;

    private void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.inventory = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadInventory();

        HideInventory();
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
        else if (inventoryUIRoot != null)
            inventoryUIRoot.gameObject.SetActive(true);
    }

    private void HideInventory()
    {
        /*
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        else if (inventoryUIRoot != null)
            inventoryUIRoot.gameObject.SetActive(false);
            */
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUIRoot == null || GameManager.Instance == null)
            return;

        foreach (Transform child in inventoryUIRoot)
            Destroy(child.gameObject);

        int totalSlots = GameManager.Instance.maxInventorySlots;

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

            InventorySlotData slotData = GameManager.Instance.GetInventorySlot(i);
            if (slotData == null || slotData.IsEmpty)
            {
                slotUI.ClearVisual();
                continue;
            }

            Item item = GameManager.Instance.FindItemInMasterList(slotData.itemID);
            if (item == null)
            {
                slotUI.ClearVisual();
                continue;
            }

            slotUI.Bind(item, slotData.quantity, rootCanvas);

            // Optional: if inventory slot prefab has a button on the root, let tap equip in equip mode
            if (pendingEquipSlot.HasValue && equipmentManager != null && item is Equipment eq)
            {
                Button button = slotObj.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();

                    if (eq.equipmentType == pendingEquipSlot.Value)
                    {
                        button.onClick.AddListener(() =>
                        {
                            equipmentManager.Equip(eq);
                            pendingEquipSlot = null;
                            HideInventory();
                        });
                    }
                }
            }
        }
    }

    public bool TryStoreGridItemInInventory(int gridIndex, int inventorySlotIndex)
    {
        if (GameManager.Instance == null)
            return false;

        CrafterGridController crafterGrid = FindObjectOfType<CrafterGridController>();
        if (crafterGrid == null)
        {
            Debug.LogWarning("No CrafterGridController found in scene.");
            return false;
        }

        bool success = crafterGrid.TryMoveGridItemToInventory(gridIndex, inventorySlotIndex);

        if (success)
            UpdateInventoryUI();

        return success;
    }
}