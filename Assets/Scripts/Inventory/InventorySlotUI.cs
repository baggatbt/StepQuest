using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IDropHandler
{
    [Header("Refs")]
    [SerializeField] private Transform itemAnchor;
    [SerializeField] private InventoryItemUI inventoryItemPrefab;

    public int SlotIndex { get; private set; }
    public Inventory Owner { get; private set; }

    private InventoryItemUI spawnedItemUI;

    public void Setup(Inventory owner, int slotIndex)
    {
        Owner = owner;
        SlotIndex = slotIndex;
    }

    public void Bind(Item item, int quantity, Canvas rootCanvas)
    {
        ClearVisual();

        if (item == null || inventoryItemPrefab == null || itemAnchor == null)
            return;

        spawnedItemUI = Instantiate(inventoryItemPrefab, itemAnchor);

        RectTransform rt = spawnedItemUI.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;

        spawnedItemUI.Setup(Owner, SlotIndex, item, quantity, rootCanvas);
    }

    public void ClearVisual()
    {
        if (spawnedItemUI != null)
            Destroy(spawnedItemUI.gameObject);

        spawnedItemUI = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Owner == null || eventData.pointerDrag == null)
            return;

        CrafterGridItemUI draggedGridItem = eventData.pointerDrag.GetComponent<CrafterGridItemUI>();
        if (draggedGridItem != null)
        {
            Owner.TryStoreGridItemInInventory(draggedGridItem.SourceIndex, SlotIndex);
            return;
        }

        // Inventory-to-inventory stack/swap can go here later.
    }
}