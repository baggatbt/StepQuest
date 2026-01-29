using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RunLootBucket
{
    [Serializable]
    public struct ItemStack
    {
        public Item item;
        public int quantity;

        public ItemStack(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }

    public int gold;
    public int xp;

    // Use a list (easy to display in UI, safe with Unity serialization patterns)
    public List<ItemStack> items = new();

    public void AddItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item != null && items[i].item.itemID == item.itemID)
            {
                var s = items[i];
                s.quantity += amount;
                items[i] = s;
                return;
            }
        }

        items.Add(new ItemStack(item, amount));
    }

    public void Clear()
    {
        gold = 0;
        xp = 0;
        items.Clear();
    }
}
