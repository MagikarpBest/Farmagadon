using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FarmTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager manager;

    private void OnEnable()
    {
        Time.timeScale = 0.0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }
    public void CloseUI()
    {
        print("E");
        manager.HideFarmTutorial();
    }
}
