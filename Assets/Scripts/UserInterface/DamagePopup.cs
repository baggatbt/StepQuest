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
    private Color startColor;

    private Vector3 startPos;
    private Vector3 endPos;

    private float timeAlive;
    private bool initialized;

    public void Setup(int damageAmount)
    {
        Init(damageAmount.ToString());
    }

    public void SetupTimingEventResult(string txt)
    {
        Init(txt);
    }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        startColor = textMesh.color;
    }

    private void Update()
    {
        if (!initialized)
            return;

        timeAlive += Time.deltaTime;
        float t = timeAlive / lifeTime;
        t = Mathf.Clamp01(t);

        float height = 4f * jumpHeight * t * (1f - t);

        float x = Mathf.Lerp(startPos.x, endPos.x, t);
        float y = Mathf.Lerp(startPos.y, endPos.y, t) + height;

        transform.localPosition = new Vector3(x, y, startPos.z);

        float scale;

        if (t < 0.1f)
        {
            scale = Mathf.Lerp(1f, popScale, t / 0.1f);
        }
        else
        {
            scale = Mathf.Lerp(popScale, 0f, (t - 0.1f) / 0.9f);
        }

        transform.localScale = Vector3.one * scale;

        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        textMesh.color = c;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private void Init(string displayText)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();

        textMesh.text = displayText;

        CaptureStartPosition();

        initialized = true;
    }

    private void CaptureStartPosition()
    {
        timeAlive = 0f;
        transform.localScale = Vector3.one;

        if (textMesh != null)
        {
            Color c = startColor;
            c.a = 1f;
            textMesh.color = c;
        }

        float xOffset = Random.Range(-horizontalDrift, horizontalDrift);

        startPos = transform.localPosition;
        endPos = startPos + new Vector3(xOffset, -jumpHeight * 0.3f, 0f);
    }
}