using UnityEngine;

public class Bullets : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] float lifeTime = 3f;

    private float damage;
    private float explosionRadius;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Called by shooter (PlayerShooting) to launch bullet
    public void Fire(Vector2 velocity, float damage, float explosionRadius)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;

        rb.linearVelocity = velocity;

        // Rotate sprite to match movement direction
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (explosionRadius > 0)
        {
            // Explosive bullet -> damage in an area
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            // Check every enemy in zone and reduce HP
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Health health = hit.GetComponent<Health>();
                    if (health != null)
                    {
                        Destroy(hit.gameObject);
                        Destroy(gameObject);
                    }
                }
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                Destroy (collision.gameObject);
                Destroy(gameObject);
            }
        }

    }
}
