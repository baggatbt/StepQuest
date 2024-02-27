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

    // Trigger the player hold release time event without waiting for damage application
    yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
    {
        Vector3 spawnPosition = target.transform.position;
        Vector3 offsetPosition = new Vector3(spawnPosition.x, spawnPosition.y + 2, spawnPosition.z);
        // Instantiate the arrow rain prefab at the offset position
        GameObject arrowRain = UnityEngine.Object.Instantiate(arrowRainFallPreFab, offsetPosition, Quaternion.identity);
    });

    // Separate loop outside the lambda expression for applying damage with delay
    for (int i = 0; i < 3; i++)
    {
        yield return new WaitForSeconds(0.2f); // Wait for 0.2 seconds between each hit
        HandleAoeAttack(user, battleManager.enemies, result, baseDamage);
    }

    yield return new WaitForSeconds(1.0f); 
    user.isAttacking = false;
}

}


       
      
        




