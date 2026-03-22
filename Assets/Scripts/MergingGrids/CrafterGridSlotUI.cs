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

        CrafterGridItemUI dragged = eventData.pointerDrag.GetComponent<CrafterGridItemUI>();
        if (dragged == null)
            return;

        Owner.TryMoveOrMerge(dragged.SourceIndex, Index);
    }
}