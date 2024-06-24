using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Battle();
        }
    }

    private void Battle()
    {
        // Your battle logic here
        Debug.Log("Battle function called!");
    }
}
