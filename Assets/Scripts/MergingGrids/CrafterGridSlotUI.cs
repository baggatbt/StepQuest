using UnityEngine;
using UnityEngine.EventSystems;

public class CrafterGridSlotUI : MonoBehaviour, IDropHandler
{
    public int Index { get; private set; }
    public CrafterGridController Owner { get; private set; }

    public void Setup(CrafterGridController owner, int index)
    {
        Owner = owner;
        Index = index;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || Owner == null)
            return;

        CrafterGridItemUI draggedGridItem = eventData.pointerDrag.GetComponent<CrafterGridItemUI>();
        if (draggedGridItem != null)
        {
            draggedGridItem.HandleSuccessfulDrop(Index);
            return;
        }

        InventoryItemUI draggedInventoryItem = eventData.pointerDrag.GetComponent<InventoryItemUI>();
        if (draggedInventoryItem != null)
        {
            bool moved = Owner.TryMoveInventoryItemToGrid(draggedInventoryItem.SlotIndex, Index);
            if (moved)
                draggedInventoryItem.HandleSuccessfulDropToGrid();

            return;
        }
    }
}