using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IInitializePotentialDragHandler
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drag Feel")]
    [SerializeField] private float dragGhostScale = 1.15f;
    [SerializeField] private Vector2 dragOffset = new Vector2(0f, 30f);

    public int SlotIndex { get; private set; }

    private Inventory owner;
    private Item itemData;
    private Canvas rootCanvas;
    private RectTransform iconRectTransform;
    private ScrollRect parentScrollRect;

    private GameObject dragGhost;
    private RectTransform dragGhostRect;

    private bool dropHandled;
    private bool isDraggingItem;

    public void Setup(Inventory inventoryOwner, int slotIndex, Item item, int quantity, Canvas canvas)
    {
        owner = inventoryOwner;
        SlotIndex = slotIndex;
        itemData = item;
        rootCanvas = canvas;
        iconRectTransform = iconImage != null ? iconImage.GetComponent<RectTransform>() : null;
        parentScrollRect = GetComponentInParent<ScrollRect>();

        dropHandled = false;
        isDraggingItem = false;

        if (iconImage != null)
        {
            iconImage.sprite = item != null ? item.itemIcon : null;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            iconImage.enabled = item != null;
        }

        if (countText != null)
        {
            countText.text = quantity > 1 ? quantity.ToString() : "";
            countText.raycastTarget = false;
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Optional later
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        // Stops the ScrollRect from hijacking the drag immediately.
        eventData.useDragThreshold = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null || rootCanvas == null || iconImage == null || iconImage.sprite == null)
            return;

        dropHandled = false;
        isDraggingItem = true;

        if (parentScrollRect != null)
            parentScrollRect.enabled = false;

        CreateDragGhost();

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }

        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingItem || dragGhostRect == null)
            return;

        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingItem)
            return;

        isDraggingItem = false;

        if (parentScrollRect != null)
            parentScrollRect.enabled = true;

        if (dropHandled)
            return;

        DestroyDragGhost();

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        owner.UpdateInventoryUI();
    }

    public void HandleSuccessfulDropToGrid()
    {
        dropHandled = true;
        isDraggingItem = false;

        if (parentScrollRect != null)
            parentScrollRect.enabled = true;

        DestroyDragGhost();

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }
    }

    private void DestroyDragGhost()
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
            dragGhostRect = null;
        }
    }

    private void CreateDragGhost()
    {
        DestroyDragGhost();

        dragGhost = Instantiate(iconImage.gameObject, rootCanvas.transform);
        dragGhost.name = $"InventoryDragGhost_{itemData.itemName}";
        dragGhost.transform.SetAsLastSibling();

        dragGhostRect = dragGhost.GetComponent<RectTransform>();

        CanvasGroup ghostCanvasGroup = dragGhost.GetComponent<CanvasGroup>();
        if (ghostCanvasGroup == null)
            ghostCanvasGroup = dragGhost.AddComponent<CanvasGroup>();

        ghostCanvasGroup.blocksRaycasts = false;
        ghostCanvasGroup.interactable = false;
        ghostCanvasGroup.alpha = 1f;

        Canvas ghostCanvas = dragGhost.GetComponent<Canvas>();
        if (ghostCanvas == null)
            ghostCanvas = dragGhost.AddComponent<Canvas>();

        ghostCanvas.overrideSorting = true;
        ghostCanvas.sortingOrder = 5000;

        GraphicRaycaster ghostRaycaster = dragGhost.GetComponent<GraphicRaycaster>();
        if (ghostRaycaster != null)
            Destroy(ghostRaycaster);

        Image ghostImage = dragGhost.GetComponent<Image>();
        if (ghostImage != null)
        {
            ghostImage.raycastTarget = false;
            ghostImage.enabled = true;
            ghostImage.color = iconImage.color;
            ghostImage.sprite = iconImage.sprite;
            ghostImage.overrideSprite = iconImage.overrideSprite;
            ghostImage.type = iconImage.type;
            ghostImage.preserveAspect = true;
        }

        Rect pixelRect = RectTransformUtility.PixelAdjustRect(iconRectTransform, rootCanvas);
        float width = pixelRect.width;
        float height = pixelRect.height;

        dragGhostRect.SetParent(rootCanvas.transform, false);
        dragGhostRect.anchorMin = new Vector2(0.5f, 0.5f);
        dragGhostRect.anchorMax = new Vector2(0.5f, 0.5f);
        dragGhostRect.pivot = new Vector2(0.5f, 0.5f);
        dragGhostRect.sizeDelta = new Vector2(width, height);
        dragGhostRect.localScale = Vector3.one * dragGhostScale;
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        if (dragGhostRect == null || rootCanvas == null)
            return;

        RectTransform canvasRect = rootCanvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            dragGhostRect.localPosition = localPoint + dragOffset;
        }
    }
}