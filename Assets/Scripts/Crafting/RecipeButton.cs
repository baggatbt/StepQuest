using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RecipeButton : MonoBehaviour
{
    [Tooltip("Drag your CraftingManager here")]
    public CraftingManager craftingManager;

    [Tooltip("Drag the Recipe asset you want this button to trigger")]
    public Recipe recipe;

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI requirementsText;        // <-- assign in Inspector
    
     public Transform requirementsContainer;
     public GameObject materialRequirementPrefab; 

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
            Debug.LogError("RecipeButton requires a Button component!");

        _button.onClick.AddListener(OnClicked);

        // Populate UI immediately
        if (nameText != null)
            nameText.text = recipe.outputItem.itemName;

        if (requirementsText != null)
            requirementsText.text = BuildRequirementsString(recipe);

        
         PopulateRequirementsIcons(recipe);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        if (craftingManager == null || recipe == null)
        {
            Debug.LogWarning("Missing CraftingManager or Recipe on RecipeButton.");
            return;
        }
        craftingManager.StartCrafting(recipe);
    }

    private string BuildRequirementsString(Recipe recipe)
    {
        // e.g. "2 × Wood\n1 × Iron Ore"
        return string.Join("\n",
            recipe.materialRequirements
                  .Select(r => $"{r.quantity} × {r.material.itemName}")
        );
    }

    
    private void PopulateRequirementsIcons(Recipe recipe)
    {
        foreach (var req in recipe.materialRequirements)
        {
            var go = Instantiate(materialRequirementPrefab, requirementsContainer);
            var img = go.GetComponentInChildren<Image>();
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            img.sprite = req.material.itemIcon;    // assuming MaterialItem has an icon
            txt.text       = req.quantity.ToString();
        }
    }
    
}
