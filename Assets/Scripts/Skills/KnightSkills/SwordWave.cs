using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordWave : Skill
{
    private GameObject projectilePrefab;

    public SwordWave()
    {
        skillName = "Sword Wave";
        description = "A ranged slash of energy.";
        energyCost = 3;
        energyGain = 1;
        energyGainBonus = 1;
        requiresMovement = false;
        noZoom = false;
        skillDamageModifier = 1.2f;
        projectilePrefab = Resources.Load<GameObject>("Prefab/Projectiles/SwordWaveProjectile");
    }

    public override IEnumerator Execute(Character user, Character target, BattleManager battleManager)
    {
        Vector2 direction = (target.transform.position - user.transform.position).normalized;
        int baseDamage = Mathf.CeilToInt(user.attackPower * skillDamageModifier);

        yield return battleManager.StartCoroutine(
            battleManager.PlayerHoldReleaseTimeEvent(0.5f, 1.2f, (TimingEventResult result) =>
            {
                GameObject proj = Object.Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
                Projectile projectile = proj.GetComponent<Projectile>();
                projectile.damage = baseDamage;
                projectile.Spawner = user;
                projectile.Target = target;
                projectile.Initialize(direction, user);

                HandlePlayerRangedAttack(user, projectile, result);
            })
        );

        yield return new WaitForSeconds(0.3f);
    }
}
