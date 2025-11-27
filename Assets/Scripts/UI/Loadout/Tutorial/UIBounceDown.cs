using DG.Tweening;
using System.Collections;
using Unity.Properties;
using UnityEngine;

public class UIBounceDown : MonoBehaviour
{
    private const int hiddenY = 1000;
    private const int shownY = 0;

    public void MoveUI()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, hiddenY);
        StartCoroutine(MoveUIDown());
    }

    public void HideUI()
    {
        StartCoroutine(MoveUIUp());
    }

    private IEnumerator MoveUIDown()
    {
        gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, shownY), 1.0f).SetEase(Ease.InOutBack);
        yield return null;
    }

    private IEnumerator MoveUIUp()
    {
        Tween moveTween = gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, hiddenY), 1.0f).SetEase(Ease.InOutBack);
        yield return moveTween.WaitForCompletion();
        enabled = false;
    }
}
