using UnityEngine;
using System.Collections;

public class RandomEnemyVisualLogic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] normalVisuals;
    [SerializeField] private Sprite[] flashVisuals;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.1f;

    private Enemy enemy;
    private int currentIndex = -1; // which visual we are using
    private Coroutine flashRoutine;

    private void OnEnable()
    {
        enemy.OnHit += FlashOnHit;
    }

    private void OnDisableEnable()
    {
        enemy.OnHit -= FlashOnHit;
    }
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning($"{name} could not find Enemy in parent!");
        }

        if (spriteRenderer == null || normalVisuals.Length == 0)
        {
            Debug.LogWarning("Missing SpriteRenderer or no visuals assigned!");
            return;
        }

        currentIndex = UnityEngine.Random.Range(0, normalVisuals.Length);
        spriteRenderer.sprite = normalVisuals[currentIndex];
    }

    /// <summary>
    /// Call this when enemy takes damage
    /// </summary>
    public void FlashOnHit()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // check if we have a flash sprite for this index
        if (flashVisuals != null && currentIndex < flashVisuals.Length && flashVisuals[currentIndex] != null)
        {
            //Debug.Log("flash sprite");
            spriteRenderer.sprite = flashVisuals[currentIndex];
        }

        yield return new WaitForSeconds(flashDuration);

        // revert to normal
        if (normalVisuals != null && currentIndex < normalVisuals.Length && normalVisuals[currentIndex] != null)
        {
            //Debug.Log("normal sprite");
            spriteRenderer.sprite = normalVisuals[currentIndex];
        }

        flashRoutine = null;
    }
}
