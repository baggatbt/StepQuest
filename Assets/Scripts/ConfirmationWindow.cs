using UnityEngine;

public class ConfirmationWindow : MonoBehaviour
{
    public GameObject confirmationPanel;    // Assign in Inspector
    public MainMenuUIManager mainMenuUI;    // Assign your UI manager that switches panels

    // Called by your "Back" button
    public void ShowConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    public void OnConfirm()
    {
        // This is where you do the actual action (close canvas, toggle panel, etc.)
        // Example: go back to main menu
        mainMenuUI.closeOverworldMapInterface();  // Replace with your actual method
        confirmationPanel.SetActive(false);
    }

    public void OnCancel()
    {
        confirmationPanel.SetActive(false);
    }
}
