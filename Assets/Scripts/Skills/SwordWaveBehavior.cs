using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWaveBehavior : MonoBehaviour
{
    public float speed = 10f; // You can adjust the speed to whatever you like.

    void Update()
    {
        // Move the sword wave to the right.
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Destroy the sword wave if it is no longer visible.
        if (!IsVisibleFromCamera())
        {
            Destroy(gameObject);
        }
    }

    private bool IsVisibleFromCamera()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        if (collider != null)
        {
            Bounds colliderBounds = collider.bounds;
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), colliderBounds);
        }

        return false;
    }
}
