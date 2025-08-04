using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftQuantityDialog : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI maxText;
    public TMP_InputField  qtyInput;
    public Button          plusButton;
    public Button          minusButton;
    public Button          confirmButton;
    public Button          cancelButton;

    private int maxQty;
    private Recipe recipe;
    private CraftingManager craftingManager;
    private Action<Recipe,int> onConfirm;

    /// <summary>
    /// Call right after Instantiate().
    /// </summary>
    public void Initialize(Recipe recipe,
                           CraftingManager cm,
                           Action<Recipe,int> onConfirm)
    {
        this.recipe         = recipe;
        this.craftingManager= cm;
        this.onConfirm      = onConfirm;

        // compute how many we *could* craft based on materials:
        maxQty = int.MaxValue;
        foreach (var req in recipe.materialRequirements)
        {
            int have = GameManager.Instance.GetItemCount(req.material);
            maxQty = Mathf.Min(maxQty, have / req.quantity);
        }
        if (maxQty == int.MaxValue) maxQty = 0;

        maxText.text     = $"Max: {maxQty}";
        qtyInput.text    = maxQty > 0 ? "1" : "0";
        confirmButton.interactable = maxQty > 0;

        plusButton.onClick    .AddListener(OnPlus);
        minusButton.onClick   .AddListener(OnMinus);
        confirmButton.onClick .AddListener(OnConfirm);
        cancelButton.onClick  .AddListener(OnCancel);
    }

    private void OnPlus()
    {
        if (int.TryParse(qtyInput.text, out int q) && q < maxQty)
            qtyInput.text = (q + 1).ToString();
    }

    private void OnMinus()
    {
        if (int.TryParse(qtyInput.text, out int q) && q > 1)
            qtyInput.text = (q - 1).ToString();
    }

    private void OnConfirm()
    {
        if (int.TryParse(qtyInput.text, out int q))
            onConfirm?.Invoke(recipe, Mathf.Clamp(q, 1, maxQty));
        Destroy(gameObject);
    }

    private void OnCancel()
    {
        Destroy(gameObject);
    }
}
