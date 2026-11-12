using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    // A layout-controlled aspect fit keeps the full destination art visible,
    // filling the frame's width without cropping, stretching, or side gutters.
    public sealed class RpgArtworkLayout : MonoBehaviour, ILayoutElement
    {
        public float aspect = 16f / 9f;
        public float padding;
        public void CalculateLayoutInputHorizontal() { }
        public void CalculateLayoutInputVertical() { }
        public float minWidth => -1;
        public float preferredWidth => -1;
        public float flexibleWidth => 1;
        public float minHeight => 0;
        public float preferredHeight => Mathf.Max(0, ((RectTransform)transform).rect.width - padding) / Mathf.Max(.1f, aspect) + padding;
        public float flexibleHeight => 0;
        public int layoutPriority => 2;
    }
}
