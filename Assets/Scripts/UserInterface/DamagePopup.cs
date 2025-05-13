using UnityEngine;
using TMPro;

/// <summary>
/// Flashy damage / text popup that
///  • pops larger,
///  • jumps up on a little arc,
///  • fades and shrinks as it falls,
///  • then destroys itself.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class DamagePopup : MonoBehaviour
{
    [Header("Motion")]
    [Tooltip("Horizontal drift (randomised ±X) in local pixels")]
    public float horizontalDrift = 30f;
    [Tooltip("Peak height of the parabola in local pixels")]
    public float jumpHeight = 80f;
    [Tooltip("Total lifetime in seconds")]
    public float lifeTime = 1.1f;

    [Header("Scale")]
    [Tooltip("Extra scale at the very beginning (1 = normal)")]
    public float popScale = 1.4f;

    private TextMeshProUGUI textMesh;
    private Color  startColor;
    private Vector3 startPos;
    private Vector3 endPos;
    private float   timeAlive;

    //───────────────────────────────────────────────────────────────
    #region Public API -------------------------------------------------

    public void Setup(int damageAmount)        => Init(damageAmount.ToString());
    public void SetupTimingEventResult(string txt) => Init(txt);

    #endregion
    //───────────────────────────────────────────────────────────────
    #region Unity lifecycle -------------------------------------------

    private void Awake()
    {
        textMesh   = GetComponent<TextMeshProUGUI>();
        startColor = textMesh.color;

        // randomise a tiny bit so multiple popups don’t overlap exactly
        float xOffset = Random.Range(-horizontalDrift, horizontalDrift);

        startPos = transform.localPosition;
        endPos   = startPos + new Vector3(xOffset, -jumpHeight * 0.3f, 0f); // ends slightly lower
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        float t = timeAlive / lifeTime;               // 0 → 1

        // 1.  Parabolic Y = 4h * t * (1‑t)
        float height = 4f * jumpHeight * t * (1f - t);
        // 2.  Interpolate X toward end X
        float x = Mathf.Lerp(startPos.x, endPos.x, t);

        transform.localPosition = new Vector3(x, startPos.y + height, 0f);

        // Scale pop: overshoot then return, finally shrink to 0
        float scale;
        if (t < .1f)                       // first 10 % of life
            scale = Mathf.Lerp(1f, popScale, t / .1f);
        else
            scale = Mathf.Lerp(popScale, 0f, (t - .1f) / .9f);

        transform.localScale = Vector3.one * scale;

        // Fade out
        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        textMesh.color = c;

        if (t >= 1f)
            Destroy(gameObject);
    }

    #endregion
    //───────────────────────────────────────────────────────────────
    #region Helpers ----------------------------------------------------

    private void Init(string displayText)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = displayText;
    }

    #endregion
}
