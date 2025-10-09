using UnityEngine;
using TMPro;

public class DescriptionManager : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;

    [TextArea(2, 5)] // Makes it editable & multi-line
    public string description;

    public void ShowDescription()
    {
        descriptionText.text = description;
    }
}
