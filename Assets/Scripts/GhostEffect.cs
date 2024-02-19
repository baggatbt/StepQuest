using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    public float ghostDelay = 0.1f;
    private float ghostDelaySeconds;
    public float ghostLifetime = 1f;
    public ObjectPool ghostPool; // Assign the object pool manager in the Inspector

    private List<SpriteRenderer> activeGhosts = new List<SpriteRenderer>();

    void Start()
    {
        ghostDelaySeconds = ghostDelay;
    }

    void Update()
    {
        if (ghostDelaySeconds > 0)
        {
            ghostDelaySeconds -= Time.deltaTime;
        }
        else
        {
            // Generate a ghost from the pool
            GameObject ghostObj = ghostPool.GetFromPool();
            SpriteRenderer ghostSprite = ghostObj.GetComponent<SpriteRenderer>();
            ghostSprite.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent
            activeGhosts.Add(ghostSprite);

            ghostDelaySeconds = ghostDelay;
        }

        // Fade all active ghosts
        for (int i = activeGhosts.Count - 1; i >= 0; i--)
        {
            SpriteRenderer ghost = activeGhosts[i];
            Color color = ghost.color;
            color.a -= Time.deltaTime / ghostLifetime;
            ghost.color = color;

            if (color.a <= 0f)
            {
                activeGhosts.RemoveAt(i);
                ghostPool.ReturnToPool(ghost.gameObject);
            }
        }
    }
     public void SetObjectPool(ObjectPool pool)
    {
        ghostPool = pool;
    }
}
