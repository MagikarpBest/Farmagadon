using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CircleTransition : MonoBehaviour
{
    [Header("Transtition")]
    [SerializeField] private RectTransform circleTransition;
    [SerializeField] private float transitionDuration = 0.8f;

    private void Update()
    {
        // Press O to open (expand)
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(OpenTransition());
        }

        // Press P to close (shrink)
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(CloseTransition());
        }
    }

    public IEnumerator OpenTransition()
    {
        circleTransition.DOScale(1f, transitionDuration).SetEase(Ease.InOutQuad).SetUpdate(true); // expand outward
        yield return new WaitForSecondsRealtime(transitionDuration);
    }

    public IEnumerator CloseTransition()
    {
        Debug.Log("close transition");
        circleTransition.DOScale(0f, transitionDuration).SetEase(Ease.InOutQuad).SetUpdate(true); // shrink inward }
        yield return new WaitForSecondsRealtime(transitionDuration);
    }

    public IEnumerator FullTransition()
    {
        yield return OpenTransition();

        yield return CloseTransition();
    }
}