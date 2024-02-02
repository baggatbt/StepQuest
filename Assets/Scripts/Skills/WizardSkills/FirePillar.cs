using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirePillar : Skill
{
    GameObject firePillarPreFab = Resources.Load<GameObject>("PreFab/Projectiles_Effects/FirePillarPrefab");


    public FirePillar()
    {
        skillName = "Fire Pillar";
        description = "Strike with a powerful column of flame";
        requiresMovement = false;
        energyCost = 2;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.8);  // 180% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        int baseDamage = CalculateBaseDamage(user);

        

        yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
        {
            
            Vector3 spawnPosition = target.transform.position;
            Vector3 offsetPosition = new Vector3(spawnPosition.x, spawnPosition.y + 2, spawnPosition.z);
            GameObject firePillar = UnityEngine.Object.Instantiate(firePillarPreFab, offsetPosition, Quaternion.identity);

           

            HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
          
        });
       
      
        
    
    
        user.isAttacking = false;
       
        
    }
}


       
      
        





