using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSanity : MonoBehaviour
{
    Camera cam;
    void Awake() { cam = GetComponent<Camera>(); }
    void LateUpdate()
    {
        Debug.Log($"[CameraSanity] {name} ortho={cam.orthographic} size={cam.orthographicSize} depth={cam.depth}");
    }
}
