using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    // Let long equipment names and dialogue choices grow vertically instead of
    // shrinking the pixel type or spilling into the next action.
    public sealed class RpgButtonLayout : MonoBehaviour, ILayoutElement
    {
        public TMP_Text label;
        public float minimumHeight, padding;
        float measured;
        public void CalculateLayoutInputHorizontal() { }
        public void CalculateLayoutInputVertical()
        {
            float width = ((RectTransform)transform).rect.width;
            measured = label == null ? minimumHeight : Mathf.Max(minimumHeight,
                label.GetPreferredValues(label.text, Mathf.Max(1, width), Mathf.Infinity).y + padding * .25f);
        }
        public float minWidth => -1;
        public float preferredWidth => -1;
        public float flexibleWidth => 1;
        public float minHeight => minimumHeight;
        public float preferredHeight => Mathf.Max(minimumHeight, measured);
        public float flexibleHeight => 0;
        public int layoutPriority => 2;
    }
}
