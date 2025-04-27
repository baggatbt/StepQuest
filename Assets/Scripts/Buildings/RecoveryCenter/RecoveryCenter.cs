using UnityEngine;

// If you have your own Building base class, inherit from it;
// otherwise replace 'Building' with 'MonoBehaviour'.
public class RecoveryCenter : Building
{
    [SerializeField] private RecoveryCenterData data;
    [SerializeField, Min(0)] private int buildingLevel = 0;
    public int StepCostCurrentLevel => data.stepCostPerStamina[buildingLevel];


    /*-----------------------------------------------------------
     *  Called by your UI when the player confirms the recovery
     *----------------------------------------------------------*/
    public bool TryRecover(Companion hero, int staminaToRestore)
    {
        int costPerPoint = data.stepCostPerStamina[buildingLevel];
        int totalSteps   = costPerPoint * staminaToRestore;

        if (!PlayerData.Instance.UseSteps(totalSteps))
            return false;                         // not enough steps

        hero.stamina = Mathf.Min(
            hero.stamina + staminaToRestore,
            data.maxStaminaPerHero[buildingLevel]);

        hero.SaveCharacterData();
        Debug.Log("Restored Stamina");
        return true;
    }

    /*-----------------------------------------------------------
     *  Passive hourly regen (QoL unlocked at higher levels)
     *----------------------------------------------------------*/
    private float nextRegenTime;

    private void Update()
    {
        float regenRate = data.autoRegenPerHour[buildingLevel];
        if (regenRate <= 0) return;

        if (Time.time >= nextRegenTime)
        {
            foreach (var h in GameManager.Instance.companions)
                h.stamina = Mathf.Min(
                    h.stamina + (int)regenRate,
                    data.maxStaminaPerHero[buildingLevel]);

            nextRegenTime = Time.time + 3600f;     // next hour
        }
    }

    public void Upgrade()                          // called by upgrade UI
    {
        if (buildingLevel < data.stepCostPerStamina.Length - 1)
            buildingLevel++;
    }
}
