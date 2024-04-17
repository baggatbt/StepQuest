using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI;
    public GameObject slotPrefab;

    private void Start()
    {
        GameManager.Instance.LoadInventory(); // Load the inventory at start
    }

    public void UpdateInventoryUI()
    {
        if (inventoryUI == null) return;

        foreach (Transform child in inventoryUI.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in GameManager.Instance.itemList)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            slot.SetActive(true);

            Image itemImage = slot.transform.Find("ItemContainer/ItemImage")?.GetComponent<Image>();
            if (itemImage != null && item.itemIcon != null)
            {
                itemImage.sprite = item.itemIcon;
            }

            Text itemCountText = slot.transform.Find("ItemCountText")?.GetComponent<Text>();
            if (itemCountText != null)
            {
                itemCountText.text = item.quantity.ToString();
            }
        }
    }
}
