[System.Serializable]
public class DropEntry
{
    public Item item;         // Reference to the item to drop
    public int minQuantity;   // Minimum amount to drop
    public int maxQuantity;   // Maximum amount to drop
    public float dropChance;  // 0 to 1 (e.g., 0.5 = 50% chance)
}

