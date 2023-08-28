using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 



public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.1f;

    public IEnumerator Shake()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;
        Debug.Log("Shakin");
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPos;
    }
}
