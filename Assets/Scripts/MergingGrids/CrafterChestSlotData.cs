using System;

[Serializable]
public class CrafterChestSlotData
{
    public CrafterEntityType storedType = CrafterEntityType.None;

    public bool IsEmpty => storedType == CrafterEntityType.None;

    public void Set(CrafterEntityType type)
    {
        storedType = type;
    }

    public void Clear()
    {
        storedType = CrafterEntityType.None;
    }
}