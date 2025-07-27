using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordWave : Skill
{
    
    GameObject swordWaveProjectile = Resources.Load<GameObject>("PreFab/SwordWaveProjectile");


    public SwordWave()
    {
        skillName = "SwordWave";
        description = "Release a powerful wave from your sword.";
        requiresMovement = false;
        energyCost = 2;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.5);  // 150% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        Projectile projectileScript = null;  // Declare projectileScript here
        int baseDamage = CalculateBaseDamage(user);

        

        yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
        {
            
            Vector3 spawnPosition = user.transform.position;
            Vector2 direction = (target.transform.position - user.transform.position).normalized;
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            GameObject swordWave = UnityEngine.Object.Instantiate(swordWaveProjectile, spawnPosition, rotation);

            // Set the value of the projectile
            projectileScript = swordWave.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.damage = baseDamage;
                projectileScript.speed = 7.5f;
                projectileScript.Spawner = user;  // Set the spawner
            }


            HandleAoeAttack(user, battleManager.enemies, result, baseDamage);
          
        });
       
        yield return new WaitUntil(() => projectileScript.isColliding == true);
        user.isAttacking = false;
        user.isAnimationDone = false;
        
        bonusGained = false;
        
    }
}


       
      
        





