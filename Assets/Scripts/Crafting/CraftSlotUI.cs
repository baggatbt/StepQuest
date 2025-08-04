using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image         iconImage;
    public Slider        progressBar;
    public TextMeshProUGUI progressText;
    public GameObject    emptyOverlay;  // e.g. a translucent X or grey background

    /// <summary>
    /// Call for a filled slot.
    /// </summary>
    public void Setup(Sprite icon, float progress, float required)
    {
        emptyOverlay.SetActive(false);
        iconImage.sprite = icon;
        progressBar.maxValue = required;
        progressBar.value    = progress;
        progressText.text    = $"{Mathf.FloorToInt(progress)}/{Mathf.FloorToInt(required)}";
    }

    /// <summary>
    /// Call for an empty slot.
    /// </summary>
    public void SetEmpty()
    {
        emptyOverlay.SetActive(true);
        iconImage.sprite = null;
        progressBar.value = 0;
        progressText.text = "";
    }
}
