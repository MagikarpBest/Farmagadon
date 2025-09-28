using UnityEngine;

public class Bullets : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float damage = 1f;
    [SerializeField] float speed = 5f;
    [SerializeField] float explosionRadius = 0f; // 0 = not explosive bullet

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
    public void Fire(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;

        // Rotate sprite to match movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If collided with stuff check isit enemy, if yes then deal damage.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
