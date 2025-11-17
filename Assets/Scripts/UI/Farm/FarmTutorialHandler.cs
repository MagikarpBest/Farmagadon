using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FarmTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager manager;
    public void CloseUI()
    {
        manager.HideFarmTutorial();
    }
}
