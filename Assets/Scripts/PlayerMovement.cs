using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public Transform movementTarget;
    public float moveSpeed = 5f;

    private void Update()
    {
        // Move the player sprite towards the movement target using Lerp
        transform.position = Vector3.Lerp(transform.position, movementTarget.position, moveSpeed * Time.deltaTime);
    }
}
