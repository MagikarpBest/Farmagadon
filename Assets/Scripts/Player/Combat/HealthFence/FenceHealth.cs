using UnityEngine;
using System;
using DG.Tweening;

public class FenceHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FlashEffect flashEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] FenceVisualManager fenceVisualManager;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Fence Sprites")]
    [SerializeField] private Sprite healthySprite;
    [SerializeField] private Sprite damaged75Sprite;
    [SerializeField] private Sprite damaged50Sprite;
    [SerializeField] private Sprite damaged25Sprite;
    [SerializeField] private Sprite destroyedSprite;

    public event Action<int, int> OnHealthChanged; // current, max
    public event Action OnFenceDestroy;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateFenceSprite();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        //Debug.Log($"{currentHealth}");
        // Trigger flash, sprite update happens after flash
        flashEffect.CallDamageFlash(
            onFlashStart: () =>
            {
                fenceVisualManager.CallHitAnimation(flashEffect.flashCooldown);
                UpdateFenceSprite();
            }
        );


        if (currentHealth <= 0)
        {
            OnFenceDestroy?.Invoke();
            Debug.Log("Fence destroyed!");
        }
    }

    private void UpdateFenceSprite()
    {
        float healthPercent = (float)currentHealth / maxHealth;
        //Debug.Log("Update fence sprite");
        if (healthPercent <= 0f)
            spriteRenderer.sprite = destroyedSprite;
        else if (healthPercent <= 0.25f)
            spriteRenderer.sprite = damaged25Sprite;
        else if (healthPercent <= 0.50f)
            spriteRenderer.sprite = damaged50Sprite;
        else if (healthPercent <= 0.75f)
            spriteRenderer.sprite = damaged75Sprite;
        else
            spriteRenderer.sprite = healthySprite;
    }

    public int GetHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
