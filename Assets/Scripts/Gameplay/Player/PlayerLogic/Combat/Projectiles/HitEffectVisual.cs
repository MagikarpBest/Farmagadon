using UnityEngine;
using DG.Tweening;

public class HitEffectVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
    }
    void OnEnable()
    {
        PlayAnimationAndDestroy();
    }

    private void PlayAnimationAndDestroy()
    {
        if (animator == null)
        {
            Debug.LogWarning("No Animator found on hit effect!", this);
            FadeOutAndDestroy();
            return;
        }

        // Get current animation duration
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animDuration = animInfo.length;

        //After animation ends > fade out > destroy
        DOVirtual.DelayedCall(animDuration, () =>
        {
            FadeOutAndDestroy();
        });
    }

    private void FadeOutAndDestroy()
    {
        if (spriteRenderer == null)
        {
            Destroy(gameObject);
            return;
        }

        // Fade out to 0 alpha over 0.2 seconds
        spriteRenderer.DOFade(0f, 0.4f).OnComplete(() => { Destroy(transform.parent.gameObject); }); 
    }

}
