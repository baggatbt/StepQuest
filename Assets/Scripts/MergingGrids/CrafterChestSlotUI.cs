using UnityEngine;
using UnityEngine.EventSystems;

public class CrafterChestSlotUI : MonoBehaviour, IDropHandler
{
    public int SlotIndex { get; private set; }
    public CrafterChestPanelUI Owner { get; private set; }

    public void Setup(CrafterChestPanelUI owner, int slotIndex)
    {
        Owner = owner;
        SlotIndex = slotIndex;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Owner == null || eventData.pointerDrag == null)
            return;

        CrafterGridItemUI draggedGridItem = eventData.pointerDrag.GetComponent<CrafterGridItemUI>();
        if (draggedGridItem != null)
        {
            draggedGridItem.HandleSuccessfulChestStore(SlotIndex);
        }
    }
}