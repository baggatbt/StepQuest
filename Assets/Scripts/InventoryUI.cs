using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class InventoryUI : MonoBehaviour {
    public Inventory inventory;
    public Transform itemsParent; // The parent object holding all the inventory slots
    private List<Slot> slots;

    private void Start() {
    slots = itemsParent.GetComponentsInChildren<Slot>().ToList();
    for (int i = 0; i < slots.Count; i++) {
        slots[i].slotIndex = i; // Assign the slot index
    }
}


    // Call this method whenever the inventory changes
    public void UpdateUI(int slotIndex) {
    if (slotIndex < 0 || slotIndex >= slots.Count) {
        Debug.LogWarning("Provided slot index is out of bounds.");
        return;
    }

    if (slotIndex < inventory.items.Count) {
        slots[slotIndex].AddItem(inventory.items[slotIndex]);
    } else {
        slots[slotIndex].ClearSlot();
    }
}

}