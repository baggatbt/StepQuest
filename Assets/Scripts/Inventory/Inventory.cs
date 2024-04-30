using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

        // Assuming 'ItemContainer' is where you want to set the image for the item
        Image itemImage = slot.transform.Find("ItemContainer")?.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.itemIcon;
            if (item.itemIcon == null)
            {
                Debug.LogError("Item sprite is null for item: " + item.itemName);
            }
        }
        else
        {
            Debug.LogError("Image component not found for ItemContainer.");
        }

        // Use TextMeshProUGUI instead of Text
        TextMeshProUGUI itemCountText = slot.transform.Find("ItemCountText")?.GetComponent<TextMeshProUGUI>();
        if (itemCountText != null)
        {
            itemCountText.text = item.quantity.ToString();
        }
        else
        {
            Debug.LogError("ItemCountText component not found for slot prefab.");
        }

        Debug.Log($"Updated UI with item: {item.itemName}, Quantity: {item.quantity}");
    }
}



}
