using System.Collections;
using DG.Tweening;
using TMPro;    
using UnityEngine;

public class LevelIntroUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Timing Settings")]
    [SerializeField] private Ease fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease fadeOutEase = Ease.InQuad;
    [SerializeField] private float fadeInDuration = 0.6f;
    [SerializeField] private float displayDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.6f;
    
    public IEnumerator PlayLevelIntro(int levelNumber, string levelName)
    {
        Debug.Log("Played level intro");

        canvasGroup.alpha = 0f;

        titleText.text = $"Level {levelNumber}\n{levelName}";

        // Let Unity update UI before tween starts
        yield return null;
        yield return canvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeInEase).WaitForCompletion();
        Debug.Log($"Tween started, initial alpha: {canvasGroup.alpha}");
        yield return new WaitForSeconds(displayDuration);
        Debug.Log($"Tween finished, final alpha: {canvasGroup.alpha}");
        yield return canvasGroup.DOFade(0f, fadeOutDuration).SetEase(fadeOutEase).WaitForCompletion();
    }
}
