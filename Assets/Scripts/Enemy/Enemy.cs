using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData enemyData;

    public event Action<Enemy> OnDeath;

    private int currentHealth;
    private bool isAttackingFence = false;
    private Coroutine attackRoutine;


    private void Awake()
    {
        currentHealth = enemyData.maxHealth;
    }

    private void Update()
    {
        // Move if not attacking
        if (!isAttackingFence)
        {
            transform.Translate(Vector2.down * enemyData.moveSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        // Enemy take damage logic
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
        // Stop attack if died
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When enemy touches fence
        FenceHealth fence = other.GetComponent<FenceHealth>();
        // Start attacking fence
        if (fence != null && !isAttackingFence) 
        {
            StartAttackingFence(fence);
        }
    }

    private void StartAttackingFence(FenceHealth fence)
    {
        isAttackingFence = true;
        attackRoutine = StartCoroutine(AttackFence(fence));
    }

    private IEnumerator AttackFence(FenceHealth fence)
    {
        while (fence != null && fence.GetHealth() > 0)
        {
            // Deal damage to fence based on enemy damage
            fence.TakeDamage(enemyData.damage);
            Debug.Log($"{enemyData.enemyName} attacks fence for {enemyData.damage}");

            // Wait attack cd time of enemy before attacking again
            yield return new WaitForSeconds(enemyData.attackInterval);
        }
        // Stop attack if fence is destroyed
        isAttackingFence = false;
        attackRoutine = null;
    }
}