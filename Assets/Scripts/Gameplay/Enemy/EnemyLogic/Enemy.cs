using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private EnemyVisualHandler enemyVisualHandler;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private FlashEffect flashEffect;           // Go to flash effect to edit on hit flash setting

    public event Action<Enemy> OnDeath;

    private Collider2D[] colliders;
    private float originalMoveSpeed;
    private float currentMoveSpeed;
    private int currentHealth;
    private bool isAttackingFence = false;
    private bool isDead = false;

    private Coroutine slowRoutine;
    private Coroutine attackRoutine;

    private void Awake()
    {
        // Save movementspeed to modify later
        originalMoveSpeed = enemyData.moveSpeed;
        currentMoveSpeed = originalMoveSpeed;

        currentHealth = enemyData.maxHealth;
        colliders = GetComponentsInChildren<Collider2D>();
    }

    private void Update()
    {
        // Move if not attacking and dead
        if (!isAttackingFence && !isDead) 
        {
            transform.Translate(Vector2.down * currentMoveSpeed * Time.deltaTime);
            enemyVisualHandler.PlayMoveAnimation();
        }
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (slowRoutine != null)
        {
            StopCoroutine(slowRoutine);
        }
        slowRoutine = StartCoroutine(SlowRoutine(slowMultiplier, duration));
    }

    private IEnumerator SlowRoutine(float slowMultiplier, float duration)
    {
        currentMoveSpeed = originalMoveSpeed * Mathf.Clamp(slowMultiplier, 0f, 1f);

        yield return new WaitForSeconds(duration);

        currentMoveSpeed = originalMoveSpeed;

        slowRoutine = null;
    }
    #region Attack
    public void TakeDamage(int damage)
    {
        if (enemyVisualHandler == null)
        {
            Debug.LogWarning($"{enemyData.enemyName} has no EnemyVisualHandler reference!", this);
        }

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
        StartCoroutine(enemyVisualHandler.PlayHitAnimation());
        Debug.Log(enemyData.enemyName + " took " + damage + " damage. HP left " + currentHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAfterDelay());
        }
    }

    private IEnumerator DieAfterDelay()
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
            StartCoroutine(enemyVisualHandler.PlayAttackAnimation(enemyData.attackInterval));

            // Wait until the hit frame
            yield return new WaitForSeconds(enemyVisualHandler.GetHitTiming(enemyData.attackInterval));
            fence.TakeDamage(enemyData.damage);

            // Wait remaining recovery time
            yield return new WaitForSeconds(enemyVisualHandler.GetRecoverTiming(enemyData.attackInterval));
            //Debug.Log($"{enemyData.enemyName} attacks fence for {enemyData.damage}");
        }
        // Stop attack if fence is destroyed
        isAttackingFence = false;
        attackRoutine = null;
    }
    #endregion Attack
}