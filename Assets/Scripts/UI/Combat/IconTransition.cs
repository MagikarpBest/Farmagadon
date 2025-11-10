using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CircleTransition : MonoBehaviour
{
    [Header("Transtition")]
    [SerializeField] private RectTransform circleTransition;
    [SerializeField] private float transitionDuration = 0.8f;

    [Header("Size Settings")]
    [SerializeField] private Vector2 openSize = new Vector2(3000, 3000);
    [SerializeField] private Vector2 closeSize = new Vector2(0,0);

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
        Debug.Log("close transition");
        circleTransition.DOSizeDelta(closeSize, transitionDuration).SetEase(Ease.InOutQuad).SetUpdate(true); // shrink inward }
        yield return new WaitForSecondsRealtime(transitionDuration);

    }

    public IEnumerator CloseTransition()
    {
        circleTransition.DOSizeDelta(openSize, transitionDuration).SetEase(Ease.InOutQuad).SetUpdate(true); // expand outward
        yield return new WaitForSecondsRealtime(transitionDuration);
    }

    public IEnumerator FullTransition()
    {
        yield return OpenTransition();

        yield return CloseTransition();
    }
}