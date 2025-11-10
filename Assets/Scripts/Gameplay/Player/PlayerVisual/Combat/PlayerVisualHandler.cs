using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerVisualHandler : MonoBehaviour
{
    [SerializeField] SpriteRenderer playerSprite;

    [Header("Visual Settings")]
    [SerializeField] float rotationSpeed = 400f;
    [SerializeField] float recoilDistance = 0.2f;
    [SerializeField] float recoilDuration = 0.1f;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = playerSprite.transform.localPosition;
    }

    public void PlayRotateAnimation(float rotation)
    {
        playerSprite.transform.Rotate(0f, 0f, rotation * rotationSpeed);
    }

    public IEnumerator PlayShootAnimation()
    {
        // Snap rotation upright smoothly and wait until complete
        yield return playerSprite.transform.DOLocalRotate(Vector3.zero, 0.25f).WaitForCompletion();

        // Bounce back up
        Vector3 recoilPosition = originalPosition + Vector3.up * recoilDistance;
        playerSprite.transform.DOLocalMove(recoilPosition, recoilDuration).SetLoops(2, LoopType.Yoyo);

        // Set back to normal position
        playerSprite.transform.localPosition = originalPosition;
        Debug.Log("PLAY SHOOT ANIMATION");
    }
}
