using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootArrow : Skill
{
    
    GameObject arrowProjectile = Resources.Load<GameObject>("PreFab/Arrow");


    public ShootArrow()
    {
        skillName = "Arrow";
        description = "Shoot an arrow";
        requiresMovement = false;
        energyCost = 0;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.0);  // 100% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        user.isAttacking = true;
        Projectile projectileScript = null;  
        int baseDamage = CalculateBaseDamage(user);

        

        yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
        {
            
            Vector3 spawnPosition = user.transform.position;
            Vector2 direction = (target.transform.position - user.transform.position).normalized;
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            GameObject arrow = UnityEngine.Object.Instantiate(arrowProjectile, spawnPosition, rotation);

            // Set the value of the projectile
            projectileScript = arrow.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.damage = baseDamage;
                projectileScript.speed = 7.5f;
                projectileScript.Spawner = user;  // Set the spawner
            }


            HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
          
        });
       
        yield return new WaitUntil(() => projectileScript.isColliding == true);
        user.isAttacking = false;
        
    }
}


       
      
        