using UnityEngine;
using System;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    [Header("References")]
    private Material[] materials;
    private SpriteRenderer[] spriteRenderers;

    [Header("Flash Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTimer = 0.1f;
    [SerializeField] public float flashCooldown = 0.15f;

    private Coroutine damageFlashCoroutine;
    private bool canFlash = true;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        materials = new Material[spriteRenderers.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }

    /// <summary>
    /// Triggers a flash effect.
    /// Optional: 
    /// onFlashStart > called immediately when flash starts.
    /// onFlashComplete > called when flash finishes.
    /// </summary>
    public void CallDamageFlash(Action onFlashStart = null, Action onFlashComplete = null)
    {
        if (!canFlash)
        {
            // Skip flash if on cooldown
            return;
        }

        if (damageFlashCoroutine == null)
        {
            //Debug.Log("Flash occur");
            damageFlashCoroutine = StartCoroutine(DamageFlasher(onFlashStart, onFlashComplete));
        }
    }

    private IEnumerator DamageFlasher(Action onFlashStart, Action onFlashComplete)
    {
        canFlash = false;

        // Immediately trigger start callback
        onFlashStart?.Invoke();

        // Set flash color
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_FlashColor", flashColor);
        }

        // Lerp flash from 1 to 0 over flashTimer
        float elapsedTime = 0f;
        while (elapsedTime < flashTimer)
        {
            elapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(elapsedTime / flashTimer);
            float flashAmount = Mathf.Lerp(1f, 0f, time);

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_FlashAmount", flashAmount);
            }

            yield return null;
        }

        // Ensure flash ends at 0
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat("_FlashAmount", 0f);
        }

        // Callback for FenceHealth or other systems
        onFlashComplete?.Invoke();

        // Wait cooldown
        yield return new WaitForSeconds(flashCooldown);

        canFlash = true;
        damageFlashCoroutine = null;
    }
}