using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FarmTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager manager;
    private FarmController farmController => FindAnyObjectByType<FarmController>();
    public void CloseUI()
    {
        manager.HideFarmTutorial();
        farmController.BeginFarmCycle(FarmController.saveData.currentLevel-2);
    }
}
