using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteFlasher : MonoBehaviour
{
    public Material flashMaterial; // Assign the white flash material in the inspector
    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material; // Store the original material
    }

    public void Flash()
    {
        Debug.Log("Flashing!");
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.material = flashMaterial; // Change to the flash material
        yield return new WaitForSeconds(0.1f); // Duration of the flash
        spriteRenderer.material = originalMaterial; // Revert to the original material
    }
}

