using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerVisualHandler : MonoBehaviour
{
    [SerializeField] GameObject playerSprite;
    [SerializeField] private Animator animator;
    [Header("Visual Settings")]
    [SerializeField] float rotationSpeed = 400f;
    [SerializeField] float recoilDistance = 0.2f;
    [SerializeField] float recoilDuration = 0.15f;

    private Vector3 originalPosition;
    private Vector3 originalScale;

    private void Awake()
    {
        originalPosition = playerSprite.transform.localPosition;
        originalScale = playerSprite.transform.localScale; 
    }

    public void PlayMoveAnimation(int direction)
    {
        if (direction == 0)
        {
            animator.SetBool("IsMovingLeft", false);
            animator.SetBool("IsMovingRight", false);
        }
        else if (direction == 1)
        {
            animator.SetBool("IsMovingLeft", false);
            animator.SetBool("IsMovingRight", true);
        }
        else
        {
            animator.SetBool("IsMovingLeft", true);
            animator.SetBool("IsMovingRight", false);
        }

    }

    public IEnumerator PlayShootAnimation()
    {
        // Trigger shooting animator
        animator.SetTrigger("IsShooting");

        // --- SUCK-IN / CHARGE PHASE ---
        // Slightly move down/back and squash
        Vector3 suckPosition = originalPosition + Vector3.down * 0.1f;
        Vector3 suckScale = originalScale * 1.1f; // slightly bigger, stretching
        Tween suckMove = playerSprite.transform.DOLocalMove(suckPosition, 0.25f).SetEase(Ease.OutSine);
        Tween suckScaleTween = playerSprite.transform.DOScale(suckScale, 0.25f).SetEase(Ease.OutSine);

        
        yield return DOTween.Sequence().Join(suckMove).Join(suckScaleTween).WaitForCompletion();


        // --- SHOOT / RECOIL PHASE ---
        // Quick upward recoil and squash/stretch to feel impact
        Vector3 recoilPosition = originalPosition + Vector3.up * recoilDistance;
        Vector3 recoilScale = originalScale * 0.9f; // slightly smaller, compressing
        Tween recoilMove = playerSprite.transform.DOLocalMove(recoilPosition, recoilDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutSine);
        Tween recoilScaleTween = playerSprite.transform.DOScale(recoilScale, recoilDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutSine);

        yield return DOTween.Sequence().Join(recoilMove).Join(recoilScaleTween).WaitForCompletion();

        // --- RESET ---
        playerSprite.transform.localPosition = originalPosition;
        playerSprite.transform.localScale = originalScale;
        playerSprite.transform.DOLocalRotate(Vector3.zero, 0.1f); // optional rotation reset


    }
}
