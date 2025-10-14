using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem.Processors;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyData enemyData;

    public event Action<Enemy> OnDeath;
    public event Action OnHit;

    private int currentHealth;
    private bool isAttackingFence = false;
    private bool isDead = false;
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
        OnHit?.Invoke();
        // Enemy take damage logic
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log(enemyData.enemyName + " took " + damage + " damage. HP left " + currentHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAfterDelay(0.1f));
        }
    }

    private IEnumerator DieAfterDelay(float delay)
    {
        if (isDead)
        {
            yield break;
        }
        isDead = true;

        Debug.Log(enemyData.enemyName + " died");

        // Stop attack if died
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        // Allow flash to show before actually destroying the object
        yield return new WaitForSeconds(delay);

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