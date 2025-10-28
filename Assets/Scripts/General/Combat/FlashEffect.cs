using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FlashEffect : MonoBehaviour
{
    [Header("Reference")]
    private Material[] material;
    private SpriteRenderer[] spriteRenderer;

    [Header("Flash Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTimer = 0.1f;
    [SerializeField] private float flashCooldown = 0.15f;
    [SerializeField] private AnimationCurve flashSpeedCurve;

    private Coroutine damageFlashCoroutine;
    private bool canFlash = true;
    private void Awake()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        Initialize();
    }

    private void Initialize()
    {
        material = new Material[spriteRenderer.Length];

        for (int i = 0; i < material.Length; i++)
        {
            material[i] = spriteRenderer[i].material;
        }
    }

    public void CallDamageFlash()
    {
        if (!canFlash)
        {
            return;
        }
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        canFlash = false;
        // Set the color
        SetFlashColor();

        // Lerp the flash amount
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashTimer)
        {
            // Iterate elapsedTime
            elapsedTime += Time.deltaTime;
             
            // Lerp the flash amount
            currentFlashAmount = Mathf.Lerp(1.0f, flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / flashTimer));
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
        SetFlashAmount(0f);
        damageFlashCoroutine = null;

        // cooldown before allowing next flash
        yield return new WaitForSeconds(flashCooldown);
        canFlash = true;
    }

    private void SetFlashColor()
    {
        // Set the color
        for (int i = 0; i < material.Length; i++)
        {
            material[i].SetColor("_FlashColor", flashColor);
        }
    }

    private void SetFlashAmount(float amount)
    {
        // Set the flash amount
        for (int i = 0; i < material.Length; i++)
        {
            material[i].SetFloat("_FlashAmount", amount);
        }
;
    }
}