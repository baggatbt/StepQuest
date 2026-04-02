using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Request Definition")]
public class CrafterRequestDefinition : ScriptableObject
{
    public CrafterEntityType requestedType;

    [Header("Quantity")]
    public int minQuantity = 2;
    public int maxQuantity = 5;

    [Header("Rewards")]
    public int rewardCopper;
    public int rewardXP;

    public int GetRandomQuantity()
    {
        return Random.Range(minQuantity, maxQuantity + 1);
    }
}