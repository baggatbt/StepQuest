using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HarvestPrompt : MonoBehaviour
{
    [Header("Wires")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button confirmButton;
    public Button cancelButton;

    private Action onConfirm;
    private Action onCancel;

    void Awake()
    {
        confirmButton.onClick.AddListener(() => { onConfirm?.Invoke(); gameObject.SetActive(false); });
        cancelButton.onClick.AddListener(() => { onCancel?.Invoke();  gameObject.SetActive(false); });
    }

    public void Open(string title, string body, Action confirm, Action cancel = null)
    {
        titleText.text = title;
        bodyText.text  = body;
        onConfirm = confirm;
        onCancel  = cancel;
        gameObject.SetActive(true);
    }
}
