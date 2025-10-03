using UnityEngine;
using System;
using System.Collections.Generic;

public static class CurrencyManager
{
    /// <summary>
    /// Returns "Copper: X" for display purposes.
    /// </summary>
    public static string GetCopperString(long totalCopper)
    {
        return "Copper: " + totalCopper;
    }

    /// <summary>
    /// Adds the specified amount of copper. Clamps at 0.
    /// </summary>
    public static void AddCopper(ref long totalCopper, long amountToAdd)
    {
        totalCopper += amountToAdd;
        if (totalCopper < 0) totalCopper = 0;
    }

    /// <summary>
    /// Attempts to spend the specified amount of copper.
    /// Returns true if successful.
    /// </summary>
    public static bool SpendCopper(ref long totalCopper, long amountToSpend)
    {
        if (amountToSpend <= 0) return true; // no-op or negative spend
        if (totalCopper >= amountToSpend)
        {
            totalCopper -= amountToSpend;
            return true;
        }
        return false;
    }

    // ---------------------------
    // Compatibility shims (optional)
    // ---------------------------

    /// <summary>
    /// DEPRECATED: multi-denomination display no longer used.
    /// Kept for compatibility; now returns "Copper: X" only.
    /// </summary>
    [Obsolete("Multi-coin display removed. Use GetCopperString(totalCopper) instead.")]
    public static string GetMultiCoinString(long totalCopper)
    {
        return GetCopperString(totalCopper);
    }

    /// <summary>
    /// DEPRECATED: denomination split removed. Returns all zeros except copper = total.
    /// </summary>
    [Obsolete("Denominations removed. Use total copper directly.")]
    public static (long diamond, long platinum, long gold, long silver, long copper)
        GetDenominationCounts(long totalCopper)
    {
        return (0L, 0L, 0L, 0L, totalCopper);
    }
}
