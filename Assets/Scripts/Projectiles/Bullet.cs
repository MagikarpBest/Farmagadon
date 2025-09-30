using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float damage;
    private float explosionRadius;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
    }

    // Called by shooter (PlayerShooting) to launch bullet
    public void Fire(Vector2 velocity, float damage, float explosionRadius,float lifeTime)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;

        rb.linearVelocity = velocity;

        // Rotate sprite to match movement direction
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy bullet after lifetime (why cause this can make some gun with shorter range without changing much code)
        Destroy(gameObject, lifeTime);
    }


    private void OnTriggerEnter2D(Collider2D other)
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
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(Mathf.RoundToInt(damage));
                    }
                }
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            // Single target
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) 
            {
                damageable.TakeDamage(Mathf.RoundToInt(damage));
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Explosive range debug tools
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
