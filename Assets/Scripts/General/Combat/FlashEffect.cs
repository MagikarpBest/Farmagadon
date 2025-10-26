using UnityEngine;
using System.Collections;

// Script from
// https://github.com/BarthaSzabolcs/Tutorial-SpriteFlash/blob/main/Assets/Scripts/FlashEffects/SimpleFlash.cs

public class FlashEffect : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private SpriteRenderer spriteRenderer; 

    [Header("Flash Settings")]
    [SerializeField] private float duration;

    private Material originalMaterial;
    private Coroutine flashRoutine;

    void Start()
    {
        // Get the material that the SpriteRenderer uses, 
        // so we can switch back to it after the flash ended.
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }
}