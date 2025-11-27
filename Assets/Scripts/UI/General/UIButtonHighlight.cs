using System.Collections;
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

    private const float zeroAlphaValue = 0.0f;
    private const float oneAlphaValue = 1.0f;

    private void Start()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
            highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, 0.0f);
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
            highlightImage.enabled = false;
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
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator OutlineFade()
    {
        yield return new WaitForEndOfFrame();
    }
}
