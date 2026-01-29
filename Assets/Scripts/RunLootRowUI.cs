using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunLootRowUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text countText;

    public void Bind(Item item, int count)
    {
        if (icon)
        {
            icon.enabled = item != null && item.itemIcon != null;
            icon.sprite = item != null ? item.itemIcon : null;
        }

        if (nameText) nameText.text = item != null ? item.itemName : "Unknown";
        if (countText) countText.text = count.ToString();
    }
}
