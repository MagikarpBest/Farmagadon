using DG.Tweening;
using UnityEngine;
using System.Collections;

public class FenceVisualManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Header("Settings")]
    [SerializeField] private float fenceHitEffectDuration = 0.5f;

    private Coroutine hitAnimationCoroutine;

    public void CallHitAnimation(float waitDuration)
    {
        if (hitAnimationCoroutine == null)
        {
            Debug.Log("Flash occur");
            hitAnimationCoroutine = StartCoroutine(HitAnimation(waitDuration));
        }
    }

    private IEnumerator HitAnimation(float waitDuration)
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        // Stop any ongoing tweens (optional safety)
        transform.DOKill();
        spriteRenderer.DOKill();

        // 1 — Small shake (impact pop)
        transform.DOShakePosition(fenceHitEffectDuration, 0.10f, 10, 90, false, true);

        // Wait until done
        yield return new WaitForSeconds(waitDuration);
        hitAnimationCoroutine = null;
    }
}
