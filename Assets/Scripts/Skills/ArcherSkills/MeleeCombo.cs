using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeCombo : Skill
{
    
    GameObject arrowProjectile = Resources.Load<GameObject>("PreFab/Projectiles_Effects/Arrow");



    public MeleeCombo()
    {
        skillName = "ComboName";
        description = "Melee combo";
        requiresMovement = true;
        energyCost = 0;
        noZoom = true;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * 1.0);  // 100% of the character's attack.
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
    user.isAttacking = true;
    user.isAnimationDone = false;  // Reset the flag at the start of each attack
    user.animator.SetTrigger("MeleeCombo");
    Projectile projectileScript = null;  
    int baseDamage = CalculateBaseDamage(user);
    

    // Find the ArrowSpawnLocation in the user's hierarchy
    Transform arrowSpawnTransform = user.transform.Find("ArrowSpawnLocation");
    if (arrowSpawnTransform == null)
    {
        Debug.LogError("ArrowSpawnLocation not found in user's hierarchy");
        yield break; // Exit the coroutine if the transform is not found
    }

     yield return TimingWindow(user, target, battleManager, 0.0f, 0.5f);
             

             HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
             Debug.Log("Timing for player attack has been handled waiting for animations");
             //Wait until the timing event happens to move on to next attack stage
             yield return new WaitUntil(() => user.animationDamageTime == true);
             
             yield return new WaitForSeconds(0.5f);

    yield return TimingWindow(user, target, battleManager, 0.0f, 0.5f);
    
        HandleTimingResultForPlayerAttack(user, target, result, baseDamage);
        // Use the position and rotation of the ArrowSpawnLocation
        Vector3 spawnPosition = arrowSpawnTransform.position;
        Vector2 direction = (target.transform.position - spawnPosition).normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        GameObject arrow = UnityEngine.Object.Instantiate(arrowProjectile, spawnPosition, rotation);
        

        // Set the value of the projectile
        projectileScript = arrow.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.damage = baseDamage;
            projectileScript.speed = 40.0f;
            projectileScript.Spawner = user;  // Set the spawner
        }
          
          // Modify the projectile's damage based on the timing result
           projectileScript.Target = target;  // Set the target of the projectile
           // HandlePlayerRangedAttack(user, projectileScript, result);
            
            
      
    
        user.animator.SetBool("ChargeIsOver", true);
        Debug.Log("waiting on animation to finish");
    
        yield return new WaitUntil(() => user.isAnimationDone == true);
        //user.GainEnergy(energyGain);
        user.animationDamageTime = false;
        user.isAnimationDone = false;
        user.isAttacking = false;
        
     
    
}


private IEnumerator TimingWindow(Character user, Character target, BattleManager battleManager, float windowStart, float windowEnd)
    {
       
        yield return battleManager.StartCoroutine(battleManager.PlayerActiveTimeEvent(windowStart, windowEnd, (timingResult) =>
        {
            result = timingResult;
        }));
         
    }

}   

