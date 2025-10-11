using UnityEngine;
using System.Collections.Generic;

public enum UIScreen
{
    HUD,
    Victory,
    GameOver,
    Pause
}

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseUI;

    private Dictionary<UIScreen, GameObject> screens;

    private void Awake()
    {
        screens = new Dictionary<UIScreen, GameObject>()
        {
            {UIScreen.HUD, hudUI },
            {UIScreen.Victory, victoryUI },
            {UIScreen.GameOver, gameOverUI },
            {UIScreen.Pause, pauseUI }
        };

        Show(UIScreen.HUD);
    }

    public void Show(UIScreen screenToShow)
    {
        foreach (var kvp in screens)
        {
            kvp.Value.SetActive(kvp.Key == screenToShow);
        }
    }

    // Optional helpers for quick access
    public void ShowHUD() => Show(UIScreen.HUD);
    public void ShowVictory() => Show(UIScreen.Victory);
    public void ShowGameOver() => Show(UIScreen.GameOver);
    public void ShowPause() => Show(UIScreen.Pause);
}
