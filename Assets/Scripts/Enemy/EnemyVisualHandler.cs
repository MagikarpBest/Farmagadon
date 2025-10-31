using UnityEngine;
using DG.Tweening;
using System.Collections;

public class EnemyVisualHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform hitEffectRoot;

    [Header("Death Settings")]
    [SerializeField] private float deathEffectDuration = 0.5f;

    [Header("Move Settings")]
    [SerializeField] private float wiggleSpeed = 15f;     // how fast it wiggles side-to-side
    [SerializeField] private float wiggleAmount = 3f;     // degrees of body rotation
    [SerializeField] private float squishSpeed = 8f;      // breathing / leg rhythm
    [SerializeField] private float squishAmount = 0.07f;  // how much it squishes vertically
    [SerializeField] private float jitterAmount = 0.02f;  // small positional jitter

    private bool supressMove = false;
    private SpriteRenderer[] spriteRenderer;
    private Vector3 basePosition;
    private Vector3 baseScale;
    private float offset;

    private void Awake()
    {
        InitializeVisuals();
    }
    private void OnEnable()
    {
        // Ensures duplicated or re-enabled enemies always re-cache
        if (spriteRenderer == null || spriteRenderer.Length == 0)
        {
            InitializeVisuals();
        }
    }

    private void Update()
    {
        if (! supressMove)
        {
            PlayMoveAnimation();
        }
    }

    private void InitializeVisuals()
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        if (hitEffectRoot == null)
        {
            hitEffectRoot = visualRoot;
        }

        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        // Cache starting transforms
        basePosition = visualRoot.localPosition;
        baseScale = visualRoot.localScale;


        // Each enemy gets slightly different timing offset for natural variation
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void PlayMoveAnimation()
    {
        float time = Time.time + offset;

        // Body wiggle rotation (like crawling legs alternating)
        float rotation = Mathf.Sin(time * wiggleSpeed) * wiggleAmount;

        // Body squish (simulate crawling rhythm)
        float scaleY = 1f - Mathf.Abs(Mathf.Sin(time * squishSpeed)) * squishAmount;
        float scaleX = 1f + Mathf.Abs(Mathf.Sin(time * squishSpeed)) * squishAmount;

        // Small random jitter for crawling imperfection
        float jitterX = Mathf.PerlinNoise(time * 3f, offset) * jitterAmount * 2f - jitterAmount;
        float jitterY = Mathf.PerlinNoise(offset, time * 3f) * jitterAmount * 2f - jitterAmount;

        visualRoot.localPosition = basePosition + new Vector3(jitterX, jitterY, 0);
        visualRoot.localRotation = Quaternion.Euler(0, 0, rotation);
        visualRoot.localScale = new Vector3(baseScale.x * scaleX, baseScale.y * scaleY, baseScale.z);
    }

    public IEnumerator PlayHitAnimation()
    {
        if (hitEffectRoot == null)
        {
            yield break;
        }

        // Stop any ongoing tweens (optional safety)
        hitEffectRoot.DOKill();

        supressMove = true;
        hitEffectRoot.DOShakePosition(0.3f, 0.15f, 10, 90, false, true);
        //visualRoot.DOPunchPosition(Vector3.one * 0.10f, 0.2f, 10, 1f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
        supressMove = false;
    }

    public IEnumerator PlayDeathAnimation()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        // Stop any ongoing tweens (optional safety)
        visualRoot.DOKill();

        // 1 — Small shake (impact pop)
        //transform.DOPunchScale(new Vector3(-0.3f, 0.3f, 0f), 0.25f, 5, 0.4f).SetEase(Ease.OutBack);
        visualRoot.DOShakePosition(0.3f, 0.15f, 10, 90, false, true);

        // 2 — Fade out + shrink
        visualRoot.DOScale(Vector3.zero, deathEffectDuration).SetEase(Ease.InBack);

        foreach (var sr in spriteRenderer)
        {
            sr.DOKill();
            sr.DOFade(0f, deathEffectDuration).SetEase(Ease.InOutSine);
        }

        yield return new WaitForSeconds(deathEffectDuration);
    }
}
