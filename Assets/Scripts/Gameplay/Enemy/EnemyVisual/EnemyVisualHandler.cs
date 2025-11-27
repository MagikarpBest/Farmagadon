using UnityEngine;
using DG.Tweening;
using System.Collections;

[System.Serializable]
public class EnemyAttackAnimationSettings
{
    [Header("Timing Ratios (sum = 1.0)")]
    [Range(0f, 1f)] public float chargeRatio = 0.3f;
    [Range(0f, 1f)] public float impactRatio = 0.1f;
    [Range(0f, 1f)] public float recoilRatio = 0.6f;

    [Header("Movement Distances")]
    public float chargeBackDistance = 0.08f;
    public float bumpDistance = 0.15f;

    [Header("Squash & Stretch")]
    [Range(0.5f, 1.5f)] public float chargeSquash = 0.9f;
    [Range(1f, 1.5f)] public float impactStretchX = 1.15f;
    [Range(0.5f, 1f)] public float impactStretchY = 0.85f;

    [Header("Easing")]
    public Ease chargeEase = Ease.InOutQuad;
    public Ease impactEase = Ease.InCubic;
    public Ease recoilEase = Ease.OutElastic;

    [Header("Elastic Settings")]
    [Range(0.5f, 2f)] public float recoilElasticity = 1.4f;
    [Range(0f, 1f)] public float recoilAmplitude = 0.3f;
}

public class EnemyVisualHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform MovementVisualRoot;
    [SerializeField] private Transform hitEffectRoot;

    [Header("Death Settings")]
    [SerializeField] private float deathEffectDuration = 0.5f;

    [Header("Move Settings")]
    [SerializeField] private float wiggleSpeed = 6f;     // how fast it wiggles side-to-side
    [SerializeField] private float wiggleAmount = 3f;     // degrees of body rotation
    [SerializeField] private float squishSpeed = 8f;      // breathing / leg rhythm
    [SerializeField] private float squishAmount = 0.07f;  // how much it squishes vertically
    [SerializeField] private float jitterAmount = 0.02f;  // small positional jitter

    [Header("Attack Animation Settings")]
    [SerializeField] private EnemyAttackAnimationSettings attackSettings;

    [Header("Enemy Getting Hit Sound")]
    [SerializeField] private AudioClip enemyHitClip;

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
        
    }

    private void InitializeVisuals()
    {
        if (MovementVisualRoot == null)
        {
            MovementVisualRoot = transform;
        }

        if (hitEffectRoot == null)
        {
            hitEffectRoot = MovementVisualRoot;
        }

        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        // Cache starting transforms
        basePosition = MovementVisualRoot.localPosition;
        baseScale = MovementVisualRoot.localScale;


        // Each enemy gets slightly different timing offset for natural variation
        offset = Random.Range(0f, Mathf.PI * 2f);
    }
        
    public void PlayMoveAnimation()
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

        MovementVisualRoot.localPosition = basePosition + new Vector3(jitterX, jitterY, 0);
        MovementVisualRoot.localRotation = Quaternion.Euler(0, 0, rotation);
        MovementVisualRoot.localScale = new Vector3(baseScale.x * scaleX, baseScale.y * scaleY, baseScale.z);
    }

    public IEnumerator PlayAttackAnimation(float attackInterval)
    {
        // Stop any ongoing tweens (optional safety)
        MovementVisualRoot.DOKill();

        // Calculate attack phase animation
        float chargeDuration = attackInterval * attackSettings.chargeRatio; // charge up
        float impactDuration = attackInterval * attackSettings.impactRatio; // Foward impact
        float recoilDuration = attackInterval * attackSettings.recoilRatio; // Big elastic recovery

        // Back and base position
        Vector3 chargeBack = basePosition + new Vector3(0, attackSettings.chargeBackDistance, 0);
        Vector3 impactFoward = basePosition + new Vector3(0, -attackSettings.bumpDistance, 0);

        Sequence attackSequence = DOTween.Sequence();

        // Charge phase
        attackSequence.Append(MovementVisualRoot.DOLocalMove(chargeBack, chargeDuration)
                      .SetEase(attackSettings.chargeEase))

                      .Join(MovementVisualRoot.DOScale(baseScale * attackSettings.chargeSquash, chargeDuration)
                      .SetEase(attackSettings.chargeEase));

        // Impact phase
        attackSequence.Append(MovementVisualRoot.DOLocalMove(impactFoward, impactDuration)
                      .SetEase(attackSettings.impactEase))

                      .Join(MovementVisualRoot.DOScale(new Vector3(
                          baseScale.x * attackSettings.impactStretchX,
                          baseScale.y * attackSettings.impactStretchY,
                          baseScale.z), impactDuration)
                      .SetEase(attackSettings.impactEase));

        // Recoil after impact
        attackSequence.Append(MovementVisualRoot.DOLocalMove(basePosition, recoilDuration)
                      .SetEase(attackSettings.recoilEase, attackSettings.recoilElasticity, attackSettings.recoilAmplitude))

                      .Join(MovementVisualRoot.DOScale(baseScale, recoilDuration)
                      .SetEase(attackSettings.recoilEase, 1.2f, 0.3f));


        attackSequence.OnKill(() =>
        {
            MovementVisualRoot.localPosition = basePosition;
            MovementVisualRoot.localScale = baseScale;
        });


        yield return new WaitForSeconds(attackInterval);
    }

    public IEnumerator PlayHitAnimation()
    {
        if (hitEffectRoot == null)
        {
            yield break;
        }
        AudioService.AudioManager.PlayOneShot(enemyHitClip, 1f);
        // Stop any ongoing tweens (optional safety)
        hitEffectRoot.DOKill();
        hitEffectRoot.localPosition = basePosition;

        hitEffectRoot.DOShakePosition(0.3f, 0.12f, 9, 90, false, true);
        //visualRoot.DOPunchPosition(Vector3.one * 0.10f, 0.2f, 10, 1f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
    }

    public IEnumerator PlayDeathAnimation()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        // Stop any ongoing tweens (optional safety)
        hitEffectRoot.DOKill();
        MovementVisualRoot.DOKill();

        // 1 — Small shake (impact pop)
        //transform.DOPunchScale(new Vector3(-0.3f, 0.3f, 0f), 0.25f, 5, 0.4f).SetEase(Ease.OutBack);
        MovementVisualRoot.DOShakePosition(0.3f, 0.15f, 10, 90, false, true);

        // 2 — Fade out + shrink
        MovementVisualRoot.DOScale(Vector3.zero, deathEffectDuration).SetEase(Ease.InBack);

        foreach (var sr in spriteRenderer)
        {
            sr.DOKill();
            sr.DOFade(0f, deathEffectDuration).SetEase(Ease.InOutSine);
        }

        yield return new WaitForSeconds(deathEffectDuration + 0.2f);
    }

    public float GetHitTiming(float attackInterval)
    {
        return attackInterval * (attackSettings.chargeRatio + attackSettings.impactRatio);
    }
    
    public float GetRecoverTiming(float attackInterval)
    {
        return attackInterval * (attackSettings.recoilRatio);
    }
}
