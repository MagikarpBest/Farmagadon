using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData enemyData;

    private int currentHealth;
    private Rigidbody2D rb;


    private void Awake()
    {
        currentHealth = enemyData.maxHealth;
    }

    private void Update()
    {
        //always move down i guess
        transform.Translate(Vector2.down * enemyData.moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(enemyData.enemyName + " took " + damage + " damage. HP left " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(enemyData.enemyName + " died");
        Destroy(gameObject);
    }
}
