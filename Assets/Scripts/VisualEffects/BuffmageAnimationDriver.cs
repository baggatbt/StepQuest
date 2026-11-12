using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BuffmageAnimationDriver : MonoBehaviour
{
    public BuffmageAnimationSet animations;
    Buffmage owner;
    Animator animator;
    BattleManager battle;
    Coroutine deathRoutine;
    float deathDeadline;
    public bool IsDying { get; private set; }
    public bool Available => animations != null && animator != null && animations.controller != null;
    public float DeathPresentationRemaining => IsDying ? Mathf.Max(0, deathDeadline - Time.unscaledTime) : 0;

    void Awake()
    {
        owner = GetComponent<Buffmage>(); animator = GetComponent<Animator>();
        if (animations == null && owner != null && owner.settings != null) animations = owner.settings.animations;
        Initialize(animations);
    }
    public void Initialize(BuffmageAnimationSet set)
    {
        if (owner == null) owner = GetComponent<Buffmage>();
        if (animator == null) animator = GetComponent<Animator>();
        if (battle == null) battle = FindFirstObjectByType<BattleManager>();
        animations = set;
        if (!Available) return;
        if (animator.runtimeAnimatorController != animations.controller) animator.runtimeAnimatorController = animations.controller;
        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }
    public bool Play(string state)
    { return PoseAt(state, 0); }
    public bool PoseAt(string state, float normalizedTime)
    {
        if (!Available || IsDying || owner == null || owner.health <= 0 || !animator.isActiveAndEnabled) return false;
        int hash = Animator.StringToHash("Base Layer." + state);
        if (!animator.HasState(0, hash)) return false;
        // Nudge contact poses past floating-point key boundaries, without
        // advancing far enough to skip a pixel frame.
        float time = normalizedTime > 0 ? normalizedTime + .0001f : 0;
        animator.Play(hash, 0, Mathf.Clamp01(time));
        animator.Update(0); return true;
    }
    public void ReturnToIdle() { Play("Idle"); }
    public void FaceTarget(Transform target)
    {
        if (owner == null || owner.spriteRenderer == null || target == null) return;
        float delta = target.position.x - transform.position.x;
        if (Mathf.Abs(delta) > .01f) owner.spriteRenderer.flipX = delta < 0;
    }
    void FaceBattleOpponent()
    {
        if (battle == null || !battle.isBattleStarted || !battle.playerParty.Contains(owner) || owner.isAttacking || IsDying) return;
        Character closest = null; float distance = float.PositiveInfinity;
        foreach (var enemy in battle.enemies)
        {
            if (enemy == null || enemy.health <= 0) continue;
            float candidate = Mathf.Abs(enemy.transform.position.x - transform.position.x);
            if (candidate < distance) { closest = enemy; distance = candidate; }
        }
        if (closest != null) FaceTarget(closest.transform);
    }
    void LateUpdate()
    {
        // Also catches lethal reflection, whose existing code changes HP directly.
        if (owner == null) return;
        FaceBattleOpponent();
        if (owner.health <= 0)
        {
            if (battle != null && battle.isBattleStarted && battle.playerParty.Contains(owner)) TryPlayDeath();
            return;
        }
        if (IsDying || (Available && animator.GetBool("Dead"))) ResetForBattle();
    }
    public bool TryPlayDeath()
    {
        if (IsDying) return true;
        if (!Available || owner == null || owner.health > 0 || !gameObject.activeInHierarchy) return false;
        IsDying = true;
        animator.SetBool("Dead", true);
        animator.ResetTrigger("IsHurtTrigger"); animator.ResetTrigger("EnemyIsHurtTrigger");
        owner.GetComponent<BuffmagePresentation>()?.ResetPresentation();
        owner.isAttacking = owner.isMoving = false;
        owner.StopAllCoroutines();
        if (owner.spriteRenderer != null) owner.spriteRenderer.color = Color.white;
        var collider = GetComponent<Collider2D>(); if (collider != null) collider.enabled = false;
        animator.Play(Animator.StringToHash("Base Layer.Death"), 0, 0);
        float duration = animations.Duration(animations.death, .8f) + Mathf.Max(0, animations.deathHoldSeconds);
        deathDeadline = Time.unscaledTime + duration + .25f;
        deathRoutine = StartCoroutine(DeathPresentation(duration)); return true;
    }
    IEnumerator DeathPresentation(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (owner == null || owner.health > 0) { ResetForBattle(); yield break; }
        // Retain the shared HP/UI cleanup after the full collapse has played.
        yield return owner.FadeOutSprite();
        deathRoutine = null;
    }
    public void ResetForBattle()
    {
        if (deathRoutine != null) { StopCoroutine(deathRoutine); deathRoutine = null; }
        IsDying = false; deathDeadline = 0;
        if (owner != null && owner.health > 0)
        {
            var collider = GetComponent<Collider2D>(); if (collider != null) collider.enabled = true;
            if (owner.spriteRenderer != null) owner.spriteRenderer.color = Color.white;
            if (Available)
            {
                animator.SetBool("Dead", false);
                animator.ResetTrigger("IsHurtTrigger"); animator.ResetTrigger("EnemyIsHurtTrigger");
                ReturnToIdle();
            }
        }
    }
    void OnEnable() { if (owner != null && owner.health > 0) ResetForBattle(); }
    void OnDisable()
    {
        if (deathRoutine != null) { StopCoroutine(deathRoutine); deathRoutine = null; }
    }
}
