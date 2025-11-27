using UnityEngine;

public class CombatTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    public void CloseUI()
    {
        uiManager.HideCombatTutorial();
        // start game here
        // some hardcode for now
        WaveManager waveManager = FindAnyObjectByType<WaveManager>();
        waveManager.BeginLevel(1);
    }
}
