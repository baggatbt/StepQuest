using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/RecoveryCenterData")]
public class RecoveryCenterData : ScriptableObject
{
    [Header("Arrays are indexed by building level (0 = base)")]
    public int[]  stepCostPerStamina;   // e.g. {200,150,120,100}
    public int[]  maxStaminaPerHero;    // e.g. {10,10,12,15}
    public float[] autoRegenPerHour;    // e.g. {0,0,1,2}
}
