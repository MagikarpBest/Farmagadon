using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Handles visual highlight for UI buttons when navigated via keyboard/controller.
/// Displays a custom highlight image instead of Unity's built-in color tint.
/// </summary>
public class UIButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("References")]
    [SerializeField] private Image highlightImage;

    private float minFadeAlpha = 0f;
    private float maxAlpha = 1.0f;
    private float minBreathAlpha = 0.3f;
    private float fadeDuration = 0.2f;
    private float breathTimer = 0.4f;

    private Tween fadeTween;
    private Tween breathTween;

    private void Start()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = true;

            Color color = highlightImage.color;
            color.a = 0f;
            highlightImage.color = color;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (highlightImage != null)
        {
            fadeTween?.Kill();
            breathTween?.Kill();

            // Fade in highlight (0 to 1)
            fadeTween = highlightImage
                .DOFade(maxAlpha, fadeDuration)
                .OnComplete(startBreathing);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (highlightImage != null)
        {
            fadeTween?.Kill();
            breathTween?.Kill();

            // Fade out highlight (1 to 0)
            fadeTween = highlightImage.DOFade(minFadeAlpha, fadeDuration);
        }
    }

    private void startBreathing()
    {
        breathTween = highlightImage
            .DOFade(minBreathAlpha, breathTimer)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}