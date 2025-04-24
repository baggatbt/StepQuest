using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Inventory : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI;
    public GameObject slotPrefab;
    public EquipmentManager equipmentManager;

    private void Start()
    {
        GameManager.Instance.LoadInventory();
    }

    public void UpdateInventoryUI()
{
    if (inventoryUI == null) return;

    foreach (Transform child in inventoryUI.transform)
    {
        Destroy(child.gameObject);
    }

    var items = GameManager.Instance.itemList;
    int totalSlots = GameManager.Instance.maxInventorySlots;

    for (int i = 0; i < totalSlots; i++)
    {
        GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
        slot.SetActive(true);

        Image itemImage = slot.transform.Find("ItemContainer")?.GetComponent<Image>();
        TextMeshProUGUI itemCountText = slot.transform.Find("ItemCountText")?.GetComponent<TextMeshProUGUI>();
        Button equipButton = slot.GetComponent<Button>();

        if (i < items.Count)
        {
            Item item = items[i];

            if (itemImage != null)
            {
                itemImage.enabled = true;
                itemImage.sprite = item.itemIcon;
            }

            if (itemCountText != null)
            {
                itemCountText.text = item.quantity.ToString();
            }

            if (equipButton != null && item is Equipment equipment)
            {
                equipButton.onClick.AddListener(() => equipmentManager.Equip(equipment));
            }
        }
        else
        {
            // Empty slot
            if (itemImage != null)
            {
                itemImage.enabled = false;
                itemImage.sprite = null;
            }

            if (itemCountText != null)
            {
                itemCountText.text = "";
            }

            if (equipButton != null)
            {
                equipButton.interactable = false;
                equipButton.onClick.RemoveAllListeners();
            }
        }
    }
}


}
