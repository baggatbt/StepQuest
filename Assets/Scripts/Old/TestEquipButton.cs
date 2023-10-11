using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestEquipButton : MonoBehaviour {
    public Equipment testEquipment;
    public EquipmentManager equipmentManager;
    public Slot slot;
    public InventoryUI inventoryUI;

    private void Start() {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickEquip);
    }

    void OnClickEquip() {
        
        int changedSlot = equipmentManager.Equip(testEquipment);
        slot.AddItem(testEquipment);
        inventoryUI.UpdateUI();

    }
}
