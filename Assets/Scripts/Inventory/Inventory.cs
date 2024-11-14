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

        foreach (Item item in GameManager.Instance.itemList)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            slot.SetActive(true);

            Image itemImage = slot.transform.Find("ItemContainer")?.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = item.itemIcon;
            }

            TextMeshProUGUI itemCountText = slot.transform.Find("ItemCountText")?.GetComponent<TextMeshProUGUI>();
            if (itemCountText != null)
            {
                itemCountText.text = item.quantity.ToString();
            }

            // Add button listener for equipping the item
            Button equipButton = slot.GetComponent<Button>();
            if (equipButton != null)
            {
                Equipment equipment = item as Equipment;
                if (equipment != null)
                {
                    equipButton.onClick.AddListener(() => equipmentManager.Equip(equipment));
                }
            }
        }
    }
}
