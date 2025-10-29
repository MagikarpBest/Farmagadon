using UnityEngine;
using DG.Tweening;
using System.Collections;

public class EnemyVisualHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float deathEffectDuration = 0.5f;

    private SpriteRenderer[] spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    public IEnumerator PlayDeathAnimation()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            // Stop any ongoing tweens (optional safety)
            // 1 — small shake (impact pop)
            transform.DOShakePosition(0.3f, 0.15f, 10, 90, false, true);

            // 2 — fade out + shrink
            spriteRenderer[i].DOFade(0f, deathEffectDuration).SetEase(Ease.InOutSine);
            transform.DOScale(Vector3.zero, deathEffectDuration).SetEase(Ease.InBack);

            // Wait until done
            yield return new WaitForSeconds(deathEffectDuration);
        }
    }
}
