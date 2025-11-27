using UnityEngine;
using DG.Tweening;

public class HitEffectVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float animDuration;
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        PlayAnimationAndDestroy();
        // Get current animation duration
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        animDuration = animInfo.length;
    }

    private void PlayAnimationAndDestroy()
    {
        if (animator == null)
        {
            Debug.LogWarning("No Animator found on hit effect!", this);
            FadeOutAndDestroy();
            return;
        }



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

        Destroy(transform.parent.gameObject, animDuration);
    }

}
