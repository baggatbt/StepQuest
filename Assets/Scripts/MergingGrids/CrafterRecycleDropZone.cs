using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrafterRecycleDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image highlightImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.red;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        CrafterGridItemUI dragged = eventData.pointerDrag.GetComponent<CrafterGridItemUI>();
        if (dragged == null)
            return;

        dragged.HandleRecycleDrop();
        ResetVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightImage != null)
            highlightImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisual();
    }

    private void ResetVisual()
    {
        if (highlightImage != null)
            highlightImage.color = normalColor;
    }
}