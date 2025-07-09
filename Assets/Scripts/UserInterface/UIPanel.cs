using UnityEngine;

/// <summary> Minimal helper for show / hide with CanvasGroup. </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    CanvasGroup g;

    void Awake() => g = GetComponent<CanvasGroup>();

    public void Show()
    {
        gameObject.SetActive(true);
        g.alpha        = 1;
        g.interactable = true;
        g.blocksRaycasts = true;
    }

    public void Hide()
    {
        g.interactable   = false;
        g.blocksRaycasts = false;
        g.alpha          = 0;
        gameObject.SetActive(false);
    }
}
