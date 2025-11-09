using UnityEngine;
using DG.Tweening;

public class CircleTransition : MonoBehaviour
{
    [Header("Transtition")]
    [SerializeField] private RectTransform circleTransition;
    [SerializeField] private float transitionDuration = 0.8f;

    private void Start()
    {

    }

    private void Update()
    {
        // Press O to open (expand)
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenTransition();
        }

        // Press P to close (shrink)
        if (Input.GetKeyDown(KeyCode.P))
        {
            CloseTransition();
        }
    }


    public void OpenTransition()
    {
        circleTransition.DOScale(1f, transitionDuration).SetEase(Ease.InOutQuad); // expand outward

    }
    public void CloseTransition()
    {
        circleTransition.DOScale(0f, transitionDuration).SetEase(Ease.InOutQuad); // shrink inward }
    }
}