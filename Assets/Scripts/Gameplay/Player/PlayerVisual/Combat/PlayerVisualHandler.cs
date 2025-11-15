using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerVisualHandler : MonoBehaviour
{
    [SerializeField] GameObject playerSprite;
    [SerializeField] private Animator animator;

    [Header("Visual Settings")]
    [SerializeField] float recoilDistance = 0.2f;
    [SerializeField] float recoilDuration = 0.15f;

    [SerializeField] AudioClip playerMoveClip;
    [SerializeField] AudioClip playerShootClip;

    private float sfxCooldown = 0.1f;
    private float lastSfxTime = -999f;
    
    private Vector3 originalPosition;
    private Vector3 originalScale;

    private void Awake()
    {
        originalPosition = playerSprite.transform.localPosition;
        originalScale = playerSprite.transform.localScale; 
    }

    public void PlayMoveAnimation(int direction)
    {
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsMovingRight", false);

        if (direction > 0)
        {
            animator.SetBool("IsMovingRight", true);
        }
        else if (direction < 0)
        {
            animator.SetBool("IsMovingLeft", true);
        }

        if (animator.GetBool("IsMovingLeft") || animator.GetBool("IsMovingRight")) 
        {
            if (Time.time - lastSfxTime >= sfxCooldown)
            {
                AudioService.AudioManager.PlayBGM(playerMoveClip, 1f);
                lastSfxTime = Time.time;
            }
        }
    }

    public IEnumerator PlayShootAnimation()
    {
        // Trigger shooting animator
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsMovingRight", false);
        animator.SetBool("IsShooting", true);

        // --- SUCK-IN / CHARGE PHASE ---
        // Slightly move down/back and squash
        Vector3 suckPosition = originalPosition + Vector3.down * 0.1f;
        Vector3 suckScale = originalScale * 1.1f; // slightly bigger, stretching

        AudioService.AudioManager.PlayOneShot(playerShootClip, 1f);

        Sequence suckSeq = DOTween.Sequence()
            .Join(playerSprite.transform.DOLocalMove(suckPosition, 0.25f).SetEase(Ease.OutSine))
            .Join(playerSprite.transform.DOScale(suckScale, 0.25f).SetEase(Ease.OutSine));
        
        yield return suckSeq.WaitForCompletion();
        
        animator.SetBool("IsShooting", false);

        // --- SHOOT / RECOIL PHASE ---
        // Quick upward recoil and squash/stretch to feel impact
        Vector3 recoilPosition = originalPosition + Vector3.up * recoilDistance;
        Vector3 recoilScale = originalScale * 0.9f; // slightly smaller, compressing

        Sequence recoilSeq = DOTween.Sequence()
            .Join(playerSprite.transform.DOLocalMove(recoilPosition, recoilDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutSine))
            .Join(playerSprite.transform.DOScale(recoilScale, recoilDuration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutSine));

        yield return recoilSeq.WaitForCompletion();

        // --- RESET ---
        playerSprite.transform.localPosition = originalPosition;
        playerSprite.transform.localScale = originalScale;
        playerSprite.transform.DOLocalRotate(Vector3.zero, 0.1f); // optional rotation reset
    }
}
