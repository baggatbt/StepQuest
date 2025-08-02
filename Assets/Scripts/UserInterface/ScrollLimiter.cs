using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollLimiter : MonoBehaviour
{
    public RectTransform content;
    public RectTransform viewport;

    private ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        UpdateScroll();
    }

    void Update()
    {
        UpdateScroll();
    }

    void UpdateScroll()
    {
        if (content == null || viewport == null) return;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        // Enable scrolling only if content is taller than the viewport
        scrollRect.vertical = contentHeight > viewportHeight;
    }
}
