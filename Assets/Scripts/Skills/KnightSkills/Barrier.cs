using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public bool IsActive { get; private set; } = true;
    public Animator animator;

    public void AbsorbDamage()
    {
        if (IsActive)
        {
            IsActive = false;
            animator.SetTrigger("BreakBarrier");
            Destroy(gameObject); // Destroy the barrier or deactivate it
        }
    }
}