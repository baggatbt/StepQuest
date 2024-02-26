using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowRain : Skill
{
    GameObject arrowRainFallPreFab = Resources.Load<GameObject>("PreFab/Projectiles_Effects/ArrowRainfall");


    public ArrowRain()
    {
        skillName = "Arrow Rain";
        description = "Rain down arrows";
        requiresMovement = false;
        energyCost = 2;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.2);  //Replace 1.2 with skillModifier
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        int baseDamage = CalculateBaseDamage(user) / 3;  // Split total damage into 3 parts

        Vector3 spawnPosition = target.transform.position;
        Vector3 offsetPosition = new Vector3(spawnPosition.x, spawnPosition.y + 2, spawnPosition.z);
        GameObject arrowRain = UnityEngine.Object.Instantiate(arrowRainFallPreFab, offsetPosition, Quaternion.identity);

        // Assume HandleTimingResultForPlayerAttack is where you apply damage.
        // Loop to apply damage 3 times with a delay
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.2f); // Wait for 0.5 seconds between each hit
            HandleTimingResultForPlayerAttack(user, target, result, baseDamage); // Apply one third of the damage
        }

        yield return new WaitForSeconds(1.0f); 
        user.isAttacking = false;
    }
}


       
      
        




