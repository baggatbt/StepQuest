using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionDropTable
{
    public List<DropEntry> drops = new List<DropEntry>();

    public List<Item> RollRewards()
    {
        List<Item> rolledItems = new List<Item>();

        foreach (var entry in drops)
        {
            if (UnityEngine.Random.value <= entry.dropChance)
            {
                int qty = UnityEngine.Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                if (entry.item != null)
                {
                    Item copy = UnityEngine.Object.Instantiate(entry.item); // Create a new instance
                    copy.quantity = qty;
                    rolledItems.Add(copy);
                }
            }
        }

        return rolledItems;
    }
}
