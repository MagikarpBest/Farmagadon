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

    private void Start()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = true;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
        }
    }
}
