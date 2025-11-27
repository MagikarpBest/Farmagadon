using UnityEngine;

public class LoadoutTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    public void CloseUI()
    {
        uiManager.HideLoadoutTutorial();
        // allow inputs here
    }
}
