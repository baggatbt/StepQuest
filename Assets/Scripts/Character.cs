using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{
    [Header("Identity")]
    public string characterIDNumber;
    public bool isSelected;

    [Header("Core Stats")]
    public int level = 1;

    public int maxHealth = 40;
    public int health = 40;

    public int maxEnergy = 3;
    public int energy = 0;

    public int attackPower = 10;
    public int defensePower = 10;
    public int speed = 10;

    [Header("UI")]
    public Slider healthBar;
    public Slider energyBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI energyText;
    public GameObject enemyHealthUI;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("State")]
    public bool isAttacking = false;
    public bool isFront;
    public bool hasNotGone;
    public bool isMoving;
    public bool attackTrigger;
    public bool animationEnded;
    public bool didBlock;

    [Header("Skills")]
    public Skill currentSkill;
    public List<Skill> skills = new List<Skill>();
    public int attacksBeforeSpecial;
    public Skill normalSkill;
    public Skill specialSkill;

    [Header("Status Effects")]
    public StatusEffectController statusEffectController;

    [Header("Barrier")]
    public Barrier currentBarrier;

    [Header("Reflection (optional)")]
    [Tooltip("0.10 = reflect 10% of damage taken back to attacker.")]
    public float damageReflectionPercentage = 0f;

    [Header("Movement Settings")]
    public float movementSpeed = 5f;
    public Transform attackTarget;
    public Vector3 originalPosition;

    private Rigidbody2D rb;

    [Header("Timing / Animation Events")]
    public bool isAnimationDone = false;
    public bool animationDamageTime = false;

    // Timing window
    public bool isWindowOpen = false;
    public bool damageApplied = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip criticalHitSound;

    [Header("Hit Visuals")]
    public float tintDuration = 0.1f;

    [Header("Death")]
    public EnemyDeathEffect enemyDeathEffect;

    protected virtual void Awake()
    {
        isSelected = false;

        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        statusEffectController = GetComponent<StatusEffectController>();
        enemyDeathEffect = GetComponent<EnemyDeathEffect>();

        // capture original position once
        if (originalPosition == Vector3.zero || originalPosition == new Vector3(10000f, 10000f, -16.20f))
        {
            originalPosition = transform.position;
        }

        // Ensure health is valid at startup
        health = Mathf.Clamp(health, 0, maxHealth);
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    protected virtual void Start()
    {
        RefreshUI();
    }

    public void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }

        if (healthText != null)
        {
            healthText.text = health.ToString();
        }

        if (energyBar != null)
        {
            energyBar.maxValue = maxEnergy;
            energyBar.value = energy;
        }

        if (energyText != null)
        {
            energyText.text = energy.ToString();
        }
    }

    // ---------------------------
    // Damage (Simple Stat System)
    // ---------------------------

    /// <summary>
    /// Applies damage after defense using a ratio-based reduction.
    /// This avoids "damage - defense" problems and avoids needing defense penetration.
    /// </summary>
    public void TakeDamage(int incomingDamage, Character attacker)
    {
        // Barrier absorbs the hit (no damage)
        if (currentBarrier != null && currentBarrier.IsActive)
        {
            currentBarrier.AbsorbDamage();
            return;
        }

        incomingDamage = Mathf.Max(0, incomingDamage);

        // Ratio-based reduction:
        // final = incoming * (incoming / (incoming + defense))
        // - If defense is 0, final ~= incoming
        // - If defense is high, damage shrinks smoothly but never to 0 unless incoming is 0
        int finalDamage = ComputeDamageAfterDefense(incomingDamage, defensePower);

        Debug.Log($"{name} takes {finalDamage} damage (incoming {incomingDamage}, DEF {defensePower})");

        // Apply health change
        health = Mathf.Max(0, health - finalDamage);
        BattleManager bm = FindObjectOfType<BattleManager>();
if (bm != null && this.CompareTag("Enemy"))
{
    bm.PopupEnemyHealthBar(this);
}

        // Enemy shared hurt trigger (keep your current behavior)
        EnemyIsHit();

        if (finalDamage >= 1)
        {
            if (!didBlock && animator != null)
            {
                animator.SetTrigger("IsHurtTrigger");
                DoKnockback(this.transform, attacker.transform);
                StartCoroutine(TintRed());
            }

            // Reflect (optional)
            if (attacker != null && damageReflectionPercentage > 0f)
            {
                int reflected = Mathf.Max(0, Mathf.CeilToInt(damageReflectionPercentage * finalDamage));
                if (reflected > 0)
                {
                    attacker.health = Mathf.Max(0, attacker.health - reflected);
                    attacker.RefreshUI();
                }
            }

            SpawnDamagePopup(finalDamage);
        }

        RefreshUI();
    }

    public static int ComputeDamageAfterDefense(int incomingDamage, int defense)
    {
        if (incomingDamage <= 0) return 0;
        defense = Mathf.Max(0, defense);

        // Multiplier in (0..1]
        float mult = incomingDamage / (incomingDamage + (float)defense);

        // Round to int, keep at least 1 if incoming was > 0
        int reduced = Mathf.RoundToInt(incomingDamage * mult);
        return Mathf.Max(1, reduced);
    }

    private void SpawnDamagePopup(int amount)
    {
        GameObject damagePopupPrefab = Resources.Load<GameObject>("PreFab/DamagePopup");
        if (damagePopupPrefab == null)
        {
            Debug.LogError("Failed to load DamagePopup prefab.");
            return;
        }

        Transform parent = null;
        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null) parent = canvasObj.transform;

        GameObject damagePopupInstance = Instantiate(
            damagePopupPrefab,
            transform.position,
            Quaternion.identity,
            parent
        );

        DamagePopup damagePopupScript = damagePopupInstance.GetComponent<DamagePopup>();
        if (damagePopupScript != null)
        {
            damagePopupScript.Setup(amount);
        }
    }

    // ---------------------------
    // Movement (unchanged behavior)
    // ---------------------------

    public IEnumerator MoveToTarget()
    {
        if (attackTarget == null)
        {
            Debug.LogWarning("No attack target assigned.");
            yield break;
        }

        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D targetCollider = attackTarget.GetComponent<Collider2D>();
        if (myCollider == null || targetCollider == null)
        {
            Debug.LogWarning("Missing Collider2D on self or target.");
            yield break;
        }

        animator.ResetTrigger("StopMovementAnimationTrigger");
        animator.SetTrigger("MovementAnimationTrigger");
        isMoving = true;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = new Vector2(attackTarget.position.x, attackTarget.position.y);

        Vector2 direction = (targetPos - currentPos).normalized;
        float myRadius = Mathf.Max(myCollider.bounds.extents.x, myCollider.bounds.extents.y);
        float targetRadius = Mathf.Max(targetCollider.bounds.extents.x, targetCollider.bounds.extents.y);
        float desiredDistance = myRadius + targetRadius;

        Vector2 finalDestination = targetPos - direction * desiredDistance;

        while (Vector2.Distance(currentPos, finalDestination) > 0.05f)
        {
            Vector2 newPos = Vector2.MoveTowards(currentPos, finalDestination, movementSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            yield return null;
            currentPos = rb.position;
        }

        transform.position = new Vector3(finalDestination.x, finalDestination.y, transform.position.z);

        animator.ResetTrigger("MovementAnimationTrigger");
        animator.SetTrigger("StopMovementAnimationTrigger");
        isMoving = false;

        yield return null;
    }

    public IEnumerator ReturnToPosition()
    {
        animator.ResetTrigger("StopMovementAnimationTrigger");
        animator.SetTrigger("MovementAnimationTrigger");
        isMoving = true;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = new Vector2(originalPosition.x, originalPosition.y);

        while (Vector2.Distance(currentPos, targetPos) > 0.1f)
        {
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, movementSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            yield return null;
            currentPos = rb.position;
        }

        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);

        animator.ResetTrigger("MovementAnimationTrigger");
        animator.SetTrigger("StopMovementAnimationTrigger");
        isMoving = false;

        yield return null;
    }

    // ---------------------------
    // Knockback + hit effects
    // ---------------------------

    public IEnumerator KnockbackVisual2D(Transform enemy, Vector2 impactDirectionXY, float distance, float duration)
    {
        Vector3 originalPos = enemy.position;
        Vector3 targetPos = originalPos + new Vector3(impactDirectionXY.x, impactDirectionXY.y, 0f) * distance;

        float half = duration * 0.5f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            float pct = Mathf.SmoothStep(0f, 1f, t / half);
            enemy.position = Vector3.Lerp(originalPos, targetPos, pct);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float pct = Mathf.SmoothStep(0f, 1f, t / half);
            enemy.position = Vector3.Lerp(targetPos, originalPos, pct);
            yield return null;
        }

        enemy.position = originalPos;
    }

    private void DoKnockback(Transform enemyTransform, Transform attackerTransform)
    {
        if (attackerTransform == null) return;

        Vector3 diff = enemyTransform.position - attackerTransform.position;
        Vector2 dir2D = new Vector2(diff.x, diff.y).normalized;

        float pushDistance = 0.25f;
        float totalTime = 0.30f;

        StartCoroutine(KnockbackVisual2D(enemyTransform, dir2D, pushDistance, totalTime));
    }

    private IEnumerator TintRed()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(tintDuration);
        spriteRenderer.color = originalColor;
    }

    // ---------------------------
    // Energy
    // ---------------------------

    public void SpendEnergy(int energySpent)
    {
        energy = Mathf.Clamp(energy - energySpent, 0, maxEnergy);
        RefreshUI();
    }

    public void GainEnergy(int energyGained)
    {
        if (energy < maxEnergy)
        {
            energy = Mathf.Clamp(energy + energyGained, 0, maxEnergy);
        }
        RefreshUI();
    }

    // ---------------------------
    // Timing window / animation events
    // ---------------------------

    public void AnimationEnded()
    {
        isAnimationDone = true;
    }

    public void animationDamageTiming()
    {
        animationDamageTime = true;
    }

    public void OpenTimingWindow()
    {
        isWindowOpen = true;
        damageApplied = false;
        Debug.Log("Timing window opened.");
    }

    public void CloseTimingWindow()
    {
        isWindowOpen = false;
        Debug.Log("Timing window closed.");
    }

    public bool CheckPlayerInput()
    {
        if (isWindowOpen && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Player clicked within the timing window!");
            return true;
        }
        return false;
    }

    // ---------------------------
    // Barrier
    // ---------------------------

    public void AddBarrier(GameObject barrierPrefab)
    {
        if (currentBarrier != null) return;

        GameObject barrierObj = Instantiate(barrierPrefab, transform.position, Quaternion.identity, transform);
        currentBarrier = barrierObj.GetComponent<Barrier>();
    }

    // ---------------------------
    // Death
    // ---------------------------

    public void CheckForDeath()
{
    Debug.Log("Checking for death");
    if (health > 0) return;

    if (this is Enemy enemy)
    {
        if (!enemy.objectiveKillRegistered)
        {
            enemy.objectiveKillRegistered = true;
            GuildObjectiveManager.Instance?.RegisterEnemyDefeated(enemy);
        }

        if (enemy.enemyDeathEffect != null)
            enemy.enemyDeathEffect.TriggerExplosion();

        StartCoroutine(enemy.FadeOutSprite());
        enemy.DropMaterial();
    }
    else if (this.gameObject.CompareTag("Companion"))
    {
        StartCoroutine(this.FadeOutSprite());
    }
}

    public IEnumerator FadeOutSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float fadeDuration = 0.25f;
        float currentTime = 0.0f;

        Color originalColor = sr.color;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, currentTime / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        gameObject.SetActive(false);

        if (healthBar != null) healthBar.gameObject.SetActive(false);
        if (energyBar != null) energyBar.gameObject.SetActive(false);
        if (enemyHealthUI != null) enemyHealthUI.SetActive(false);

        yield return new WaitForSeconds(0.5f);
    }

    // ---------------------------
    // Anim triggers + audio
    // ---------------------------

    public void EnemyIsHit()
    {
        if (animator != null)
            animator.SetTrigger("EnemyIsHurtTrigger");
    }

    public void PlayHitSound()
    {
        if (audioSource == null || hitSound == null) return;
        audioSource.clip = hitSound;
        audioSource.Play();
    }

    public void PlayCriticalHitSound()
    {
        if (audioSource == null || criticalHitSound == null) return;
        audioSource.clip = criticalHitSound;
        audioSource.Play();
    }

    // ---------------------------
    // Extension point
    // ---------------------------

    public virtual void UpdateStats()
    {
        // Each enemy implements its own growth
    }

    // ---------------------------
    // (Optional) time effects you had
    // ---------------------------

    public IEnumerator HitStop(float freezeDuration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(freezeDuration);
        Time.timeScale = 1f;
    }

    public void SlowTime(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void NormalizeTime()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
    }
}