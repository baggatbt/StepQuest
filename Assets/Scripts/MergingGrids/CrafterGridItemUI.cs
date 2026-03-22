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
    private CrafterEntityDefinition definition;
    private Canvas rootCanvas;
    private bool canDrag;

    private RectTransform iconRectTransform;

    private GameObject dragGhost;
    private RectTransform dragGhostRect;

    private bool dropHandled;

    public void Setup(
        CrafterGridController gridOwner,
        int sourceIndex,
        CrafterEntityDefinition entityDef,
        Canvas canvas)
    {
        owner = gridOwner;
        SourceIndex = sourceIndex;
        definition = entityDef;
        rootCanvas = canvas;
        iconRectTransform = iconImage != null ? iconImage.GetComponent<RectTransform>() : null;

        canDrag = definition != null && definition.isMovable && !definition.isGenerator && !definition.isEnemy;
        dropHandled = false;

        if (labelText != null)
            labelText.text = definition != null ? definition.displayName : "";

        if (iconImage != null)
        {
            iconImage.sprite = definition != null ? definition.iconSprite : null;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
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
        if (!canDrag || rootCanvas == null || iconImage == null || iconImage.sprite == null)
            return;

        dropHandled = false;
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

        // If a valid drop already happened, do nothing here.
        // The drop handler already cleaned up and refreshed.
        if (dropHandled)
            return;

        DestroyDragGhost();

        // Invalid drop: restore the original visual
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        owner.RefreshVisuals();
    }

    public void HandleSuccessfulDrop(int targetIndex)
    {
        if (!canDrag)
            return;

        dropHandled = true;

        DestroyDragGhost();

        // Keep original hidden so we don't briefly see a duplicate.
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
        }

        owner.TryMoveOrMerge(SourceIndex, targetIndex);
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
        dragGhost.name = $"DragGhost_{definition.displayName}";
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