using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FarmTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager manager;
    private bool isClosed;
    private FarmController farmController => FindAnyObjectByType<FarmController>();
    public void CloseUI()
    {
        if (isClosed == true)
        {
            return;
        }
        isClosed = true;

        manager.HideFarmTutorial();
        Debug.Log("trigger close ui");
        farmController.BeginFarmCycle(FarmController.saveData.currentLevel-2);
    }
}
