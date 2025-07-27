using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Character Spawner { get; set; }
    public Character Target { get; set; }
    public int damage;
    public bool isColliding;

    private Vector2 moveDirection;

    // Optional: call this if you're not setting transform.rotation manually
    public void Initialize(Vector2 direction, Character spawner)
    {
        moveDirection = direction.normalized;
        Spawner = spawner;
    }

    private void Update()
    {
        if (moveDirection == Vector2.zero)
            moveDirection = transform.right; // fallback to facing direction

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile collided");

        Character hit = other.GetComponent<Character>();
        if (!isColliding && hit != null && hit == Target)
        {
            isColliding = true;
            Target.TakeDamage(damage, Spawner);
            Target.CheckForDeath();
            Destroy(gameObject);
        }
    }
}
