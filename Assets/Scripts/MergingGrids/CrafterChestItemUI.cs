using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrafterChestItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;

    public int SlotIndex { get; private set; }

    private CrafterChestPanelUI owner;
    private CrafterEntityDefinition definition;

    public void Setup(
        CrafterChestPanelUI panelOwner,
        int slotIndex,
        CrafterEntityDefinition entityDef)
    {
        owner = panelOwner;
        SlotIndex = slotIndex;
        definition = entityDef;

        if (iconImage != null)
        {
            iconImage.sprite = definition != null ? definition.iconSprite : null;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = true;
            iconImage.enabled = definition != null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner?.TryReturnChestItemToGrid(SlotIndex);
    }
}