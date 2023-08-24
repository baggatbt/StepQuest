using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.2f;
    public Transform cameraTransform;

    float initialDuration;
    bool isShaking = false;

    private void Awake()
    {
        if (cameraTransform == null)
        {
            cameraTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        initialDuration = shakeDuration;
    }

    public void TriggerShake()
    {
        isShaking = true;
    }

    void Update()
    {
        if (isShaking)
        {
            if (shakeDuration > 0)
            {
                cameraTransform.localPosition = cameraTransform.position + Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime;
            }
            else
            {
                isShaking = false;
                shakeDuration = initialDuration;
                cameraTransform.localPosition = Vector3.zero;
            }
        }
    }
}
