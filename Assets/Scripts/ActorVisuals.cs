using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-500)] // cache before most other scripts
public class ActorVisuals : MonoBehaviour
{
    [Header("Assign your Visual child here (root of the renderers/Animator)")]
    public Transform visual;        // child with SpriteRenderer + Animator

    [Header("Optional: where others should land on you")]
    public Transform contactFront;  // set on the Player (empty child slightly in front)

    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Animator animator;

    void Reset()         { EnsureCached(); }  // when you add the component
    void OnValidate()    { EnsureCached(); }  // in prefab editor changes
    void Awake()         { EnsureCached(); }  // at runtime

    public void EnsureCached()
    {
        // 1) Find Visual if not set
        if (visual == null)
        {
            var t = transform.Find("Visual");
            if (t == null && transform.childCount > 0) t = transform.GetChild(0);
            visual = t != null ? t : transform;
        }

        // 2) Grab components (includeInactive = true so disabled visuals still work)
        if (sr == null)
            sr = visual.GetComponentInChildren<SpriteRenderer>(true);
        if (animator == null)
            animator = visual.GetComponentInChildren<Animator>(true);
    }
}
