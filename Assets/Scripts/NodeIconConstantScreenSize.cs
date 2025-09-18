// NodeIconConstantScreenSize.cs
using UnityEngine;
[ExecuteAlways]
public class NodeIconConstantScreenSize : MonoBehaviour
{
    public Camera cam;
    public float referenceOrthoSize = 20f;
    public Vector3 referenceScale = Vector3.one;
    void Reset() { cam = Camera.main; referenceScale = transform.localScale; }
    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        if (!cam || !cam.orthographic) return;
        float k = referenceOrthoSize / Mathf.Max(0.0001f, cam.orthographicSize);
        transform.localScale = referenceScale * k;
    }
}
