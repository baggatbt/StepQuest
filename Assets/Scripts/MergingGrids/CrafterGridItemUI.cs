using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrafterGridItemUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drag Feel")]
    [SerializeField] private float dragGhostScale = 1.15f;
    [SerializeField] private Vector2 dragOffset = new Vector2(0f, 30f);

    public int SourceIndex { get; private set; }

    private CrafterGridController owner;
    private CrafterEntityType entityType;
    private CrafterEntityDefinition definition;
    private Canvas rootCanvas;
    private bool canDrag;
    private RectTransform rectTransform;

    private GameObject dragGhost;
    private RectTransform dragGhostRect;
    private Image dragGhostImage;
    private CanvasGroup dragGhostCanvasGroup;

    public void Setup(
        CrafterGridController gridOwner,
        int sourceIndex,
        CrafterEntityDefinition entityDef,
        Canvas canvas)
    {
        owner = gridOwner;
        SourceIndex = sourceIndex;
        definition = entityDef;
        entityType = entityDef != null ? entityDef.entityType : CrafterEntityType.None;
        rootCanvas = canvas;
        rectTransform = GetComponent<RectTransform>();

        canDrag = definition != null && definition.isMovable && !definition.isGenerator && !definition.isEnemy;

        if (labelText != null)
            labelText.text = definition != null ? definition.displayName : entityType.ToString();

        if (iconImage != null)
        {
            iconImage.sprite = definition != null ? definition.iconSprite : null;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner?.OnEntityClicked(SourceIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag || rootCanvas == null)
            return;

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
        if (!canDrag || dragGhostRect == null)
            return;

        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        if (dragGhost != null)
            Destroy(dragGhost);

        owner.RefreshVisuals();
    }

    private void CreateDragGhost()
    {
        if (dragGhost != null)
            Destroy(dragGhost);

        dragGhost = new GameObject(
            $"DragGhost_{entityType}",
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(Image)
        );

        dragGhost.transform.SetParent(rootCanvas.transform, false);
        dragGhost.transform.SetAsLastSibling();

        dragGhostRect = dragGhost.GetComponent<RectTransform>();
        dragGhostCanvasGroup = dragGhost.GetComponent<CanvasGroup>();
        dragGhostImage = dragGhost.GetComponent<Image>();

        dragGhostCanvasGroup.blocksRaycasts = false;
        dragGhostCanvasGroup.interactable = false;
        dragGhostCanvasGroup.alpha = 1f;

        dragGhostImage.raycastTarget = false;
        dragGhostImage.sprite = iconImage != null ? iconImage.sprite : null;
        dragGhostImage.color = Color.white;
        dragGhostImage.preserveAspect = true;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

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