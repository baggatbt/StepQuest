using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sprite poses live in clips; this layer owns props and world-space travel.
// Skills supply the impact callback, so no animation event can double-hit.
public sealed class BuffmagePresentation : MonoBehaviour
{
    struct PunchRequest { public Transform target; public int count; public Action impact; }
    readonly Queue<PunchRequest> punches = new Queue<PunchRequest>();
    BuffmageSettings config;
    BuffmageAnimationDriver driver;
    Vector3 origin;
    Quaternion rotation;
    GameObject book;
    SpriteRenderer bookSprite, flame;
    Coroutine punch;
    bool active, punchRunning;
    float windupStarted, landingRecovery;
    float direction = 1;
    Sprite impactFrame;
    bool Animated => driver != null && driver.Available;
    BuffmageAnimationSet Clips => Animated ? driver.animations : null;
    float VisualScale => Animated ? Clips.authoredWorldHeight / (163f / 56f) : 1;
    Vector3 HandOffset(float x, float y) => new Vector3(x * direction * VisualScale, y * VisualScale, -.1f);

    public void Begin(BuffmageSettings settings, Transform target = null)
    {
        ResetPresentation(); config = settings; driver = GetComponent<BuffmageAnimationDriver>();
        driver?.FaceTarget(target);
        var renderer = GetComponent<Character>().spriteRenderer;
        direction = renderer != null && renderer.flipX ? -1 : 1;
        origin = transform.position; rotation = transform.rotation; active = true;
        if (impactFrame == null && config.impactSprite != null)
        {
            var source = config.impactSprite; var frame = config.impactFrame;
            impactFrame = frame.z > 0 && frame.w > 0 && frame.x >= 0 && frame.y >= 0
                && frame.x < source.rect.width && frame.y < source.rect.height
                ? Sprite.Create(source.texture, new Rect(source.rect.x + frame.x, source.rect.y + frame.y,
                    Mathf.Min(frame.z, source.rect.width - frame.x), Mathf.Min(frame.w, source.rect.height - frame.y)),
                    Vector2.one * .5f, 100, 0, SpriteMeshType.FullRect) : source;
        }
    }
    public void PrepareBook()
    {
        windupStarted = Time.unscaledTime;
        if (Animated) driver.Play("BookWindup");
        if (config.bookSprite == null) return;
        book = new GameObject("A highly flammable spellbook");
        book.transform.SetParent(transform, false);
        book.transform.localPosition = HandOffset(.3f, 1.25f);
        bookSprite = book.AddComponent<SpriteRenderer>(); bookSprite.sprite = config.bookSprite;
        CopySorting(bookSprite, 3);
        // The held book is drawn in windup/hold frames. Only the flame is a
        // separate prop until the release frame, when the projectile appears.
        bookSprite.enabled = !Animated;
        float size = Mathf.Max(.01f, bookSprite.sprite.bounds.size.y);
        book.transform.localScale = Vector3.one * (.5f * VisualScale / size);
        bookSprite.flipX = direction < 0;
        if (config.flameFrames != null && config.flameFrames.Length > 0)
        {
            var fire = new GameObject("This counts as fire magic"); fire.transform.SetParent(book.transform, false);
            flame = fire.AddComponent<SpriteRenderer>(); flame.sprite = config.flameFrames[0]; CopySorting(flame, 4);
            fire.transform.localPosition = new Vector3(0, size * .4f, -.01f);
            fire.transform.localScale = Vector3.one * (size / Mathf.Max(.01f, flame.sprite.bounds.size.y));
        }
    }
    void CopySorting(SpriteRenderer renderer, int offset)
    {
        var owner = GetComponent<Character>().spriteRenderer;
        if (owner == null) return;
        renderer.sortingLayerID = owner.sortingLayerID; renderer.sortingOrder = owner.sortingOrder + offset;
    }
    public void ChargeBook(float progress)
    {
        if (!active || Animated) return;
        transform.rotation = rotation * Quaternion.Euler(0, 0, Mathf.Round(-8 * progress));
        if (book != null) book.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(progress * -50));
    }
    void Update()
    {
        if (flame != null && config != null && config.flameFrames != null && config.flameFrames.Length > 0)
            flame.sprite = config.flameFrames[(int)(Time.unscaledTime * 10) % config.flameFrames.Length];
        if (active && Animated && book != null && book.transform.parent == transform && !bookSprite.enabled)
        {
            float duration = Clips.Duration(Clips.bookWindup, .4f);
            book.transform.localPosition = Vector3.Lerp(HandOffset(.3f, 1.25f),
                HandOffset(-.75f, 2.1f), Mathf.Clamp01((Time.unscaledTime - windupStarted) / duration));
        }
    }
    IEnumerator WaitActive(float seconds)
    {
        float deadline = Time.unscaledTime + Mathf.Clamp(seconds, 0, 10);
        while (active && Time.unscaledTime < deadline) yield return null;
    }
    public IEnumerator ThrowBook(Transform target, bool overcharged)
    {
        transform.rotation = rotation;
        if (Animated)
        {
            driver.Play("BookThrow");
            yield return WaitActive(Mathf.Clamp(Clips.bookReleaseSeconds, 0, Clips.Duration(Clips.bookThrow, .42f)));
        }
        if (!active || book == null || target == null) yield break;
        if (Animated) driver.PoseAt("BookThrow", Clips.bookReleaseSeconds / Clips.Duration(Clips.bookThrow, .42f));
        book.transform.localPosition = HandOffset(1.05f, 1.85f);
        bookSprite.enabled = true; book.transform.SetParent(null, true);
        Vector3 start = book.transform.position;
        Vector3 end = target.position + Vector3.up * 1.2f;
        float duration = Mathf.Clamp(config.throwSeconds, .1f, 2);
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            if (!active || book == null || target == null) yield break;
            float u = Mathf.Clamp01(t / duration);
            book.transform.position = Vector3.Lerp(start, end, u) + Vector3.up * (4 * u * (1 - u) * .7f);
            book.transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(u * -540));
            if (overcharged) book.transform.localScale *= 1 - Time.unscaledDeltaTime * .4f;
            yield return null;
        }
        Destroy(book); book = null; bookSprite = null;
    }
    public void Punch(Transform target, int count, Action onImpact)
    {
        if (!active || target == null || punches.Count >= 30) return;
        punches.Enqueue(new PunchRequest { target = target, count = count, impact = onImpact });
        if (!punchRunning) { punchRunning = true; punch = StartCoroutine(PunchSequence()); }
    }
    IEnumerator PunchSequence()
    {
        try
        {
            while (active && punches.Count > 0)
            {
                var request = punches.Dequeue();
                if (request.target == null) continue;
                bool left = request.count % 2 != 0;
                float duration = Animated ? Clips.Duration(left ? Clips.punchLeft : Clips.punchRight, .17f)
                    : Mathf.Clamp(config.punchSeconds, .06f, .4f);
                float strike = Animated ? (left ? Clips.leftPunchImpactSeconds : Clips.rightPunchImpactSeconds) : duration * .5f;
                strike = Mathf.Clamp(strike, .001f, duration);
                if (Animated) driver.Play(left ? "PunchLeft" : "PunchRight");
                float reachDistance = Animated ? Clips.punchReach : .9f;
                Vector3 contact = request.target.position + Vector3.left * reachDistance * direction;
                bool delivered = false;
                for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
                {
                    if (!active || request.target == null) yield break;
                    float reach = t <= strike ? t / strike : (duration - t) / Mathf.Max(.001f, duration - strike);
                    transform.position = Vector3.Lerp(origin, contact, Mathf.Clamp01(reach));
                    if (!Animated) transform.rotation = rotation * Quaternion.Euler(0, 0, left ? 6 : -6);
                    if (!delivered && t >= strike)
                    {
                        if (Animated) driver.PoseAt(left ? "PunchLeft" : "PunchRight", strike / duration);
                        delivered = true; request.impact?.Invoke();
                    }
                    yield return null;
                }
                if (active && !delivered && request.target != null)
                {
                    if (Animated) driver.PoseAt(left ? "PunchLeft" : "PunchRight", strike / duration);
                    request.impact?.Invoke();
                }
                transform.position = origin; transform.rotation = rotation;
            }
        }
        finally
        {
            punchRunning = false; punch = null;
            if (active) { transform.position = origin; transform.rotation = rotation; driver?.ReturnToIdle(); }
        }
    }
    public IEnumerator WaitForPunches()
    { while (active && punchRunning) yield return null; }

    public void PrepareMeteor() { if (Animated) driver.Play("MeteorReady"); }
    public IEnumerator LeapOnto(Transform target)
    {
        if (target == null) yield break;
        float duration = Mathf.Clamp(config.leapSeconds, .1f, 2);
        float launch = Animated ? Mathf.Min(duration * .45f, Clips.Duration(Clips.meteorLaunch, .2f)) : 0;
        float slamStart = Animated ? Mathf.Max(launch, duration - Clips.meteorImpactSeconds) : duration;
        landingRecovery = Animated ? Mathf.Max(0, Clips.Duration(Clips.meteorSlam, .25f) - Clips.meteorImpactSeconds)
            + Clips.Duration(Clips.meteorRecover, .2f) : .3f;
        if (Animated) driver.Play("MeteorLaunch");
        bool airborne = false, slamming = false;
        Vector3 end = target.position + Vector3.left * .7f * direction;
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            if (!active || target == null) yield break;
            if (Animated && !airborne && t >= launch) { airborne = true; driver.Play("MeteorAir"); }
            if (Animated && !slamming && t >= slamStart) { slamming = true; driver.Play("MeteorSlam"); }
            float u = Mathf.Clamp01(t / duration);
            transform.position = Vector3.Lerp(origin, end, u) + Vector3.up * (4 * u * (1 - u) * config.leapHeight);
            if (!Animated) transform.rotation = rotation * Quaternion.Euler(0, 0, Mathf.Round(u * -360));
            yield return null;
        }
        if (Animated) driver.PoseAt("MeteorSlam", Clips.meteorImpactSeconds / Clips.Duration(Clips.meteorSlam, .25f));
        transform.position = end; transform.rotation = rotation;
    }
    public IEnumerator LandAndRecover() { yield return WaitActive(landingRecovery); }
    public void Impact(Transform target, bool perfect)
    {
        if (target == null || impactFrame == null) return;
        var effect = new GameObject("Physical spell impact"); effect.transform.position = target.position + Vector3.up * 1.2f;
        var sprite = effect.AddComponent<SpriteRenderer>(); sprite.sprite = impactFrame; CopySorting(sprite, 5);
        effect.transform.localScale = Vector3.one * ((perfect ? 1.2f : .8f) / Mathf.Max(.01f, sprite.sprite.bounds.size.y));
        Destroy(effect, .18f);
    }
    public void ResetPresentation()
    {
        if (punch != null) StopCoroutine(punch);
        punch = null; punchRunning = false; punches.Clear();
        if (book != null) Destroy(book);
        book = null; bookSprite = flame = null;
        if (active) { transform.position = origin; transform.rotation = rotation; }
        active = false; driver?.ReturnToIdle();
    }
    void OnDisable() { ResetPresentation(); }
    void OnDestroy()
    {
        ResetPresentation();
        if (impactFrame != null && config != null && impactFrame != config.impactSprite) Destroy(impactFrame);
    }
}
