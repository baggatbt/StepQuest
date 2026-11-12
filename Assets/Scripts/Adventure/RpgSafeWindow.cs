using UnityEngine;

namespace StepQuest.Adventure
{
    // Intersect the reserved home-content area with the device's actual safe area.
    // Top resources and bottom navigation retain their existing scene positions.
    public sealed class RpgSafeWindow : MonoBehaviour
    {
        Rect previousSafe;
        Vector2 previousSize;
        void OnEnable() { Fit(); }
        void LateUpdate()
        {
            var parent = transform.parent as RectTransform;
            if (parent != null && (previousSafe != Screen.safeArea || previousSize != parent.rect.size)) Fit();
        }
        void Fit()
        {
            var rect = transform as RectTransform;
            var parent = transform.parent as RectTransform;
            var canvas = GetComponentInParent<Canvas>();
            if (parent == null || canvas == null || parent.rect.width <= 0 || parent.rect.height <= 0) return;
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            Rect safe = Screen.safeArea;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, safe.min, camera, out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, safe.max, camera, out var max);
            min = (min - parent.rect.min) / parent.rect.size;
            max = (max - parent.rect.min) / parent.rect.size;
            rect.anchorMin = Vector2.Max(new Vector2(.035f, .14f), min + Vector2.one * .012f);
            rect.anchorMax = Vector2.Min(new Vector2(.965f, .855f), max - Vector2.one * .012f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            previousSafe = safe; previousSize = parent.rect.size;
        }
    }
}
