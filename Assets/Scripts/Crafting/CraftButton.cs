using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CraftButton : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your CraftingManager here")]
    public CraftingManager craftingManager;

    [Tooltip("Drag the CraftableItem recipe asset you want this button to craft")]
    public CraftableItem craftableItem;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("CraftButton requires a Button component on the same GameObject.");
            return;
        }

        button.onClick.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        if (craftingManager == null)
        {
            Debug.LogWarning("CraftingManager is not assigned on CraftButton.");
            return;
        }

        if (craftableItem == null)
        {
            Debug.LogWarning("CraftableItem is not assigned on CraftButton.");
            return;
        }

        craftingManager.OnCraftButtonClicked(craftableItem);
    }
}