using UnityEngine;

public class MapBoundsClamper : MonoBehaviour
{
    public RectTransform mapRectTransform; // Assign the RectTransform of your map content
    public RectTransform viewportRectTransform; // Assign the RectTransform of your viewport

    void LateUpdate()
    {
        Vector3 viewWorldMin = viewportRectTransform.TransformPoint(new Vector3(0.5f, 0.5f, 0f)); // Center in viewport space
        Vector3 viewWorldMax = viewportRectTransform.TransformPoint(new Vector3(0.5f, 0.5f, 0f)); // Center in viewport space
        Vector3 contentMin = mapRectTransform.TransformPoint(mapRectTransform.rect.min);
        Vector3 contentMax = mapRectTransform.TransformPoint(mapRectTransform.rect.max);

        Vector2 newPosition = mapRectTransform.anchoredPosition;

        if (contentMin.x > viewWorldMin.x)
            newPosition.x += viewWorldMin.x - contentMin.x;
        if (contentMin.y > viewWorldMin.y)
            newPosition.y += viewWorldMin.y - contentMin.y;
        if (contentMax.x < viewWorldMax.x)
            newPosition.x += viewWorldMax.x - contentMax.x;
        if (contentMax.y < viewWorldMax.y)
            newPosition.y += viewWorldMax.y - contentMax.y;

        mapRectTransform.anchoredPosition = newPosition;
    }
}
