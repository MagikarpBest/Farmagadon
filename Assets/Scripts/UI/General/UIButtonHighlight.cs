using DG.Tweening;
using System.Collections;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles visual highlight for UI buttons when navigated via keyboard/controller.
/// Displays a custom highlight image instead of Unity's built-in color tint.
/// </summary>
public class UIButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("References")]
    [SerializeField] private Image highlightImage;
    [SerializeField] private GameObject highlightImageObject;

    private const float zeroAlphaValue = 0.5f;
    private const float oneAlphaValue = 1.0f;
    private bool outlineShowing;
    private bool inhale = false;

    private void Start()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
            highlightImage.DOFade(zeroAlphaValue, 0.0f);
            outlineShowing = false;
        }
        if (highlightImageObject != null)
        {
            highlightImageObject.SetActive(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = true;
            outlineShowing = true;
            StartCoroutine(OutlineBreathe());
        }
        if (highlightImageObject != null)
        {
            highlightImageObject.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (highlightImage != null)
        {
            //highlightImage.enabled = false;
            outlineShowing = false;
            StopCoroutine(OutlineBreathe());
            StartCoroutine(OutlineFade());
        }

        if (highlightImageObject != null)
        {
            highlightImageObject.SetActive(false);
        }
    }

    private IEnumerator OutlineBreathe()
    {
        Tween tween;
        while(outlineShowing)
        {
            if (!inhale)
            {
                inhale = true;
                tween = highlightImage.DOFade(oneAlphaValue, 0.5f);
            }
            else
            {
                inhale = false;
                tween = highlightImage.DOFade(zeroAlphaValue, 0.5f);
            }
            yield return tween.WaitForCompletion();

        }
        inhale = false;
        highlightImage.DOFade(zeroAlphaValue, 0.0f);
        yield return null;
    }

    private IEnumerator OutlineFade()
    {

        Tween fadeTween = highlightImage.DOFade(0.0f, 0.2f);
        yield return fadeTween.WaitForCompletion();
        highlightImage.enabled = false;
    }
}
