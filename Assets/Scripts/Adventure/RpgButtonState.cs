using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    public sealed class RpgButtonState : MonoBehaviour
    {
        Button button;
        TMP_Text label;
        Color normal, disabled;
        bool? previous;
        public void Initialize(Button target, TMP_Text text, Color normalColor, Color disabledColor)
        { button = target; label = text; normal = normalColor; disabled = disabledColor; Refresh(); }
        void LateUpdate() { Refresh(); }
        void Refresh()
        {
            if (button == null || label == null) return;
            bool active = button.IsInteractable();
            if (previous == active) return;
            label.color = active ? normal : disabled; previous = active;
        }
    }
}
