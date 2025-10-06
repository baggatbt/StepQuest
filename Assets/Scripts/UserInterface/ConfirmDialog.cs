using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple reusable confirmation dialog.
/// Make a prefab or scene object with:
///  - Root Panel (this script on it)
///  - Title (TMP)
///  - Message (TMP)
///  - Confirm Button (Button)
///  - Cancel Button (Button)
/// Wire the fields in the Inspector, and ensure only ONE exists at runtime.
/// </summary>
public class ConfirmDialog : MonoBehaviour
{
    public static ConfirmDialog Instance { get; private set; }

    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    private Action _onConfirm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        HideImmediate();

        // safety wiring
        if (confirmButton) confirmButton.onClick.AddListener(OnConfirmPressed);
        if (cancelButton)  cancelButton.onClick.AddListener(Hide);
    }

    public void Show(string title, string message, string confirmLabel = "Yes", string cancelLabel = "No", Action onConfirm = null)
    {
        _onConfirm = onConfirm;

        if (titleText)   titleText.text = title ?? "";
        if (messageText) messageText.text = message ?? "";

        // Update button labels if you want separate TMPs on them:
        var confirmLabelTMP = confirmButton ? confirmButton.GetComponentInChildren<TextMeshProUGUI>() : null;
        var cancelLabelTMP  = cancelButton  ? cancelButton.GetComponentInChildren<TextMeshProUGUI>()  : null;
        if (confirmLabelTMP) confirmLabelTMP.text = confirmLabel;
        if (cancelLabelTMP)  cancelLabelTMP.text  = cancelLabel;

        SetVisible(true);
    }

    public void Hide()
    {
        _onConfirm = null;
        SetVisible(false);
    }

    private void HideImmediate()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    private void OnConfirmPressed()
    {
        var cb = _onConfirm;
        Hide();
        cb?.Invoke();
    }
}
