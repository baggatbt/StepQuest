using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("Copper UI")]
    [SerializeField] private GameObject copperGroup;
    [SerializeField] private TMP_Text copperText;

    private void Update()
    {
        if (PlayerData.Instance == null)
            return;

        // Just show total copper directly
        copperText.text = PlayerData.Instance.totalCopper.ToString();

        // Always show copper group (optional: hide if zero)
        copperGroup.SetActive(true);
        // If you want it hidden at zero, use:
        // copperGroup.SetActive(PlayerData.Instance.totalCopper > 0);
    }
}
