using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float enemySpeed = 1.0f;
    private Rigidbody2D rb;
    private EnemySpawn manager;

    public void SetManager(EnemySpawn mgr)
    {
        manager = mgr;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        rb.linearVelocity = new Vector2(0.0f, -enemySpeed);
    }

    private void Update()
    {
       
    }

    private void OnCollisionEnter2d(Collider2D collision)
    {
        if (collision.gameObject.name == "Bullet")
        {
            Die();
        }
    }

    private void Die()
    {
            manager.RemoveEnemy(gameObject);
    }


}
