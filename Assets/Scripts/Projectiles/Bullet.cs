using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float damage;
    private float explosionRadius;
    private int pierceCount;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called by shooter (PlayerShooting) to launch bullet
    public void Fire(Vector2 velocity, float damage, float explosionRadius, float lifeTime, int pierceCount = 0)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        this.pierceCount = pierceCount;

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
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            // Single target
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) 
            {
                damageable.TakeDamage(Mathf.RoundToInt(damage));
            }

            if (pierceCount > 0)
            {
                pierceCount--;
                if (pierceCount <= 0)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Explosive range debug tools
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
