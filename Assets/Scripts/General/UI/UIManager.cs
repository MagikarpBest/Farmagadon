using UnityEngine;
using System.Collections.Generic;
using System;

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
    [SerializeField] private GameObject weaponChoiceUI;

    public event Action OnVictoryCompleted;
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

    /// <summary>
    /// What UI to show and hide other that is not selected
    /// </summary>
    /// <param name="screenToShow"></param>
    public void Show(UIScreen screenToShow)
    {
        foreach (var kvp in screens)
        {
            kvp.Value.SetActive(kvp.Key == screenToShow);
        }
    }

    public void PlayerPressedVictoryNext()
    {
        victoryUI.SetActive(false);  // Hide victory panel
        OnVictoryCompleted?.Invoke();// Fire event so GameManager knows
    }

    // Optional helpers for quick access
    public void ShowHUD() => Show(UIScreen.HUD);
    public void ShowVictory() => Show(UIScreen.Victory);
    public void ShowGameOver() => Show(UIScreen.GameOver);
    public void ShowPause() => Show(UIScreen.Pause);
}
