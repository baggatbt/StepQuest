using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootArrow : Skill
{
    
    GameObject arrowProjectile = Resources.Load<GameObject>("PreFab/Projectiles_Effects/Arrow");



    public ShootArrow()
    {
        skillName = "Arrow";
        description = "Hold and release with good timing to fire an arrow";
        requiresMovement = false;
        energyCost = 0;
        iconImage = LoadIconImage("SkillIcons/attack3");
        skillDamageModifier = 1.0f;
    }

    // Override the default base damage calculation.
    protected override int CalculateBaseDamage(Character user)
    {
        return (int)(user.attackPower * skillDamageModifier);  
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
{
    user.isAttacking = true;
    Projectile projectileScript = null;
    int baseDamage = CalculateBaseDamage(user);
    user.animator.SetBool("isHoldOver", false);
    user.animator.SetTrigger("IsHolding");

    Transform arrowSpawnTransform = user.transform.Find("ArrowSpawnLocation");
    if (arrowSpawnTransform == null)
    {
        Debug.LogError("ArrowSpawnLocation not found in user's hierarchy");
        yield break;
    }

    yield return battleManager.PlayerHoldReleaseTimeEvent(0.0f, 1.0f, (result) =>
    {
        user.animator.SetBool("isHoldOver", true);
        Vector3 spawnPos = arrowSpawnTransform.position;
        Vector2 direction = (target.transform.position - spawnPos).normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        GameObject arrow = Object.Instantiate(arrowProjectile, spawnPos, rotation);
        projectileScript = arrow.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.damage = baseDamage;
            projectileScript.Spawner = user;
            projectileScript.Target = target;
            projectileScript.speed = 40f;
            projectileScript.Initialize(direction, user);

            HandlePlayerRangedAttack(user, projectileScript, result);
        }
    });

    yield return new WaitUntil(() => user.isAnimationDone);
    user.animationDamageTime = false;
    user.isAnimationDone = false;
    user.isAttacking = false;

}

}