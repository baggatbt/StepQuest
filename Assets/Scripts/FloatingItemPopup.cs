using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public class FloatingItemPopup : MonoBehaviour
{
    [Tooltip("How fast it rises (pixels/sec)")]
    public float floatSpeed =  50f;
    [Tooltip("How long before it disappears")]
    public float lifetime   =  1f;

    CanvasGroup cg;
    Image       img;
    Vector3     startPos;

    void Awake()
    {
        cg       = GetComponent<CanvasGroup>();
        img      = GetComponent<Image>();
        startPos = transform.position;
    }

    /// <summary>
    /// Call immediately after Instantiate().
    /// </summary>
    public void Initialize(Sprite itemIcon)
    {
        img.sprite = itemIcon;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;
        while (elapsed < lifetime)
        {
            // move upward
            transform.position = startPos + Vector3.up * floatSpeed * (elapsed / lifetime);
            // fade out
            cg.alpha = 1f - (elapsed / lifetime);

            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
