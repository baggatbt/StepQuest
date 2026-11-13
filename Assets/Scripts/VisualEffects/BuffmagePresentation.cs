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
    SpriteRenderer bookSprite;
    readonly List<SpriteRenderer> flames = new List<SpriteRenderer>();
    Coroutine punch;
    bool active, punchRunning;
    float landingRecovery;
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
        if (Animated) driver.Play("BookWindup");
        if (config.bookSprite == null) return;
        book = new GameObject("A highly flammable spellbook");
        book.transform.SetParent(transform, false);
        book.transform.localPosition = HandOffset(.3f, 1.25f);
        bookSprite = book.AddComponent<SpriteRenderer>(); bookSprite.sprite = config.bookSprite;
        CopySorting(bookSprite, 3);
        // Overlay the small book in the authored pose with a readable spellbook.
        bookSprite.enabled = true;
        float size = Mathf.Max(.01f, bookSprite.sprite.bounds.size.y);
        book.transform.localScale = Vector3.one * (Mathf.Max(.5f, config.bookHeight) / size);
        bookSprite.flipX = direction < 0;
        AttachBookToHand();
        if (config.flameFrames != null && config.flameFrames.Length > 0)
        {
            float width = bookSprite.sprite.bounds.size.x;
            for (int i = 0; i < 3; i++)
            {
                var fire = new GameObject("Burning page edge"); fire.transform.SetParent(book.transform, false);
                var tongue = fire.AddComponent<SpriteRenderer>();
                tongue.sprite = config.flameFrames[(i * 3) % config.flameFrames.Length]; CopySorting(tongue, 4);
                flames.Add(tongue);
                fire.transform.localPosition = new Vector3((i - 1) * width * .3f, size * .35f, -.01f);
                fire.transform.localScale = Vector3.one * (size * config.bookFlameScale * (i == 1 ? 1 : .8f)
                    / Mathf.Max(.01f, tongue.sprite.bounds.size.y));
            }
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
        if (config != null && config.flameFrames != null && config.flameFrames.Length > 0)
            for (int i = 0; i < flames.Count; i++)
                if (flames[i] != null) flames[i].sprite = config.flameFrames[((int)(Time.unscaledTime * 12) + i * 3) % config.flameFrames.Length];
    }
    // Animator updates the sprite before LateUpdate. Follow that exact pose,
    // including its pivot and mirroring, instead of interpolating above the head.
    void LateUpdate()
    {
        if (active && book != null && book.transform.parent == transform) AttachBookToHand();
    }
    void AttachBookToHand()
    {
        if (!Animated || book == null || bookSprite == null) return;
        var owner = GetComponent<Character>().spriteRenderer;
        var pose = owner != null ? owner.sprite : null;
        const string prefix = "Buffmage_Book_";
        if (pose == null || !pose.name.StartsWith(prefix)
            || !int.TryParse(pose.name.Substring(prefix.Length), out int index)
            || Clips.bookGripPixels == null || index >= Clips.bookGripPixels.Length) return;
        Vector2 pixel = Clips.bookGripPixels[index];
        Vector3 hand = new Vector3((pixel.x - pose.pivot.x) / pose.pixelsPerUnit,
            (pose.rect.height - pixel.y - pose.pivot.y) / pose.pixelsPerUnit, 0);
        if (owner.flipX) hand.x = -hand.x;
        if (owner.flipY) hand.y = -hand.y;
        // Grip near the bottom of the cover: the book rests in the fist.
        Vector3 grip = bookSprite.sprite.bounds.center + Vector3.down * bookSprite.sprite.bounds.size.y * .35f;
        Vector3 offset = book.transform.TransformVector(grip);
        book.transform.position = owner.transform.TransformPoint(hand) - offset + Vector3.back * .1f;
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
        AttachBookToHand();
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
        book.transform.position = end;
        var burst = new GameObject("Spellbook burns apart");
        burst.transform.position = end;
        burst.AddComponent<BuffmageBookBurst>().Play(config, bookSprite);
        Destroy(book); book = null; bookSprite = null; flames.Clear();
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
        book = null; bookSprite = null; flames.Clear();
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

// Owns its short lifetime so the burn finishes after the skill returns to idle.
// Uses the actual book's quarters and existing pixel flames, not smooth particles.
public sealed class BuffmageBookBurst : MonoBehaviour
{
    readonly List<Sprite> fragments = new List<Sprite>();
    public void Play(BuffmageSettings settings, SpriteRenderer source)
    { StartCoroutine(Burn(settings, source.sprite, source.sortingLayerID, source.sortingOrder)); }

    IEnumerator Burn(BuffmageSettings settings, Sprite book, int layer, int order)
    {
        const int count = 10;
        var pieces = new SpriteRenderer[count];
        var fireFrames = settings.flameFrames;
        bool hasFire = fireFrames != null && fireFrames.Length > 0;
        float duration = Mathf.Clamp(settings.bookBurnSeconds, .25f, 2);
        float radius = Mathf.Max(.5f, settings.bookBurstRadius);
        for (int i = 0; i < count; i++)
        {
            var part = new GameObject(i < 4 ? "Scorched book fragment" : "Bursting ember");
            part.transform.SetParent(transform, false);
            var renderer = pieces[i] = part.AddComponent<SpriteRenderer>();
            renderer.sortingLayerID = layer; renderer.sortingOrder = order + 1;
            if (i < 4)
            {
                var rect = book.rect;
                float halfW = Mathf.Floor(rect.width / 2), halfH = Mathf.Floor(rect.height / 2);
                var fragment = Sprite.Create(book.texture,
                    new Rect(rect.x + (i % 2) * halfW, rect.y + (i / 2) * halfH, halfW, halfH),
                    Vector2.one * .5f, book.pixelsPerUnit, 0, SpriteMeshType.FullRect);
                fragments.Add(fragment); renderer.sprite = fragment;
            }
            else renderer.sprite = hasFire ? fireFrames[0] : book;
        }
        for (float elapsed = 0; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            float u = elapsed / duration;
            // Deliberately stepped motion/scale keeps the burst crisp and chunky.
            float step = Mathf.Floor(u * 12) / 12;
            for (int i = 0; i < count; i++)
            {
                var sprite = pieces[i];
                float angle = (i * 137.5f + 20) * Mathf.Deg2Rad;
                sprite.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius * step,
                    Mathf.Sin(angle) * radius * step + step * .65f, -.02f);
                sprite.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Floor(step * 8) * (i % 2 == 0 ? 45 : -45));
                if (i >= 4 && hasFire) sprite.sprite = fireFrames[(i + (int)(elapsed * 14)) % fireFrames.Length];
                float height = i < 4 ? settings.bookHeight * .55f : settings.bookHeight * (i == 4 ? 1.9f : .8f);
                sprite.transform.localScale = Vector3.one * (height * (1 - step) / Mathf.Max(.01f, sprite.sprite.bounds.size.y));
                if (i < 4) sprite.color = Color.Lerp(Color.white, new Color(.22f, .12f, .08f), Mathf.Clamp01(u * 2));
                sprite.enabled = u < (i < 4 ? .7f : 1);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
    void OnDestroy() { foreach (var sprite in fragments) if (sprite != null) Destroy(sprite); }
}
