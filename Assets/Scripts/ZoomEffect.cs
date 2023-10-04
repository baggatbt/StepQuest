
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ZoomEffect : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomFactor = 0.6f; // The degree to which the camera zooms in
    public float zoomDuration = 1.0f; // How long the zoom will last

    public float zoomOutDuration = 0.3f;
    private float initialSize;
    private Vector3 initialCameraPosition;

    private void Start()
    {
        initialSize = mainCamera.orthographicSize;
    }

    public IEnumerator ZoomCameraEffect(Vector3 targetPosition)
    {
        Debug.Log("Zoom Duration: " + zoomDuration);

        initialCameraPosition = mainCamera.transform.position; // Capture the original position
        Vector3 targetCameraPosition = new Vector3(targetPosition.x, targetPosition.y, initialCameraPosition.z);
        
        float originalSize = mainCamera.orthographicSize;
        float targetSize = originalSize * zoomFactor;

        // Smoothly zoom in and pan to target
        for (float t = 0; t < 1; t += Time.deltaTime / zoomDuration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(originalSize, targetSize, t);
            mainCamera.transform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition, t);
            yield return null;
        }
    }

    public IEnumerator ZoomOutEffect()
    {
        float originalSize = mainCamera.orthographicSize;

        // Smoothly zoom out and pan back to initial position
        for (float t = 0; t < 1; t += Time.deltaTime / zoomOutDuration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(originalSize, initialSize, t);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, initialCameraPosition, t);
            yield return null;
        }

        mainCamera.orthographicSize = initialSize;
        mainCamera.transform.position = initialCameraPosition;
    }
}
