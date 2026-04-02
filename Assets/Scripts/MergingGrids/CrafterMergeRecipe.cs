using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Crafter Merge Recipe")]
public class CrafterMergeRecipe : ScriptableObject
{
    [Header("Inputs")]
    public CrafterEntityType inputA;
    public CrafterEntityType inputB;

    [Header("Output")]
    public CrafterEntityType result;

    public bool Matches(CrafterEntityType a, CrafterEntityType b)
    {
        bool directMatch = inputA == a && inputB == b;
        bool reverseMatch = inputA == b && inputB == a;
        return directMatch || reverseMatch;
    }
}