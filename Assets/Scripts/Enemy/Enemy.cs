using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private EnemyVisualHandler enemyVisualHandler;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private FlashEffect flashEffect;           // Go to flash effect to edit on hit flash setting

    [Header("Tween settings")]
    [SerializeField] float fadeDuration = 0.5f;

    public event Action<Enemy> OnDeath;
    public event Action OnHit;

    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderer;
    private int currentHealth;
    private bool isAttackingFence = false;
    private bool isDead = false;
    private Coroutine attackRoutine;
    private void Awake()
    {
        currentHealth = enemyData.maxHealth;
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    private void Update()
    {
        // Move if not attacking and dead
        if (!isAttackingFence && !isDead) 
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

        if (flashEffect == null)
        {
            Debug.LogWarning("Flash effect is null");
        }

        currentHealth -= damage;
        flashEffect.CallDamageFlash();
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

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // Stop attack if died
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        if (enemyVisualHandler != null)
        {
            yield return enemyVisualHandler.PlayDeathAnimation();
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
            //Debug.Log($"{enemyData.enemyName} attacks fence for {enemyData.damage}");

            // Wait attack cd time of enemy before attacking again
            yield return new WaitForSeconds(enemyData.attackInterval);
        }
        // Stop attack if fence is destroyed
        isAttackingFence = false;
        attackRoutine = null;
    }
}