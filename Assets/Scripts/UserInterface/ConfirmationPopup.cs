using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmationPopup : MonoBehaviour
{
    public GameObject panel;
    public Button confirmButton;
    public Button denyButton;
    public Text messageText; // Ensure you have a Text element for messages

    private Action onConfirmAction; // Delegate to store the Confirm action

    void Start()
    {
        panel.SetActive(false);

        confirmButton.onClick.AddListener(() => {
            onConfirmAction?.Invoke(); // Invoke the action
            Hide();
        });

        denyButton.onClick.AddListener(Hide);
    }

    // Method to show the popup and set the specific action to confirm
    public void Show(string message, Action confirmAction)
    {
        messageText.text = message; // Set the message text dynamically
        onConfirmAction = confirmAction; // Set the action to perform on confirmation
        panel.SetActive(true);
    }

    // Hide the popup
    private void Hide()
    {
        panel.SetActive(false);
    }
}