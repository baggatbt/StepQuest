using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("Coin Groups (Parent GameObjects)")]
    [SerializeField] private GameObject diamondGroup;
    [SerializeField] private GameObject platinumGroup;
    [SerializeField] private GameObject goldGroup;
    [SerializeField] private GameObject silverGroup;
    [SerializeField] private GameObject copperGroup;

    [Header("Coin Count Texts (TMP_Text)")]
    [SerializeField] private TMP_Text diamondText;
    [SerializeField] private TMP_Text platinumText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text silverText;
    [SerializeField] private TMP_Text copperText;

    private void Update()
    {
        if (PlayerData.Instance == null)
            return;

        // Get the count for each coin type from totalCopper.
        var counts = CurrencyManager.GetDenominationCounts(PlayerData.Instance.totalCopper);

        // Update each TMP text field with the whole number.
        diamondText.text  = counts.diamond.ToString();
        platinumText.text = counts.platinum.ToString();
        goldText.text     = counts.gold.ToString();
        silverText.text   = counts.silver.ToString();
        copperText.text   = counts.copper.ToString();

        // Hide any coin group if the count is less than 1.
        // If you always want to show copper, simply leave copperGroup active.
        diamondGroup.SetActive(counts.diamond > 0);
        platinumGroup.SetActive(counts.platinum > 0);
        goldGroup.SetActive(counts.gold > 0);
        silverGroup.SetActive(counts.silver > 0);
        // Uncomment the following line if you want to hide copper when zero.
        // Otherwise, always show copper:
        // copperGroup.SetActive(counts.copper > 0);
        copperGroup.SetActive(true);
    }
}
