using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Device;

public enum UIScreen
{
    HUD,
    Victory,
    GameOver,
    Pause,
}

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject hudPanel;           // Main HUD for both Farm & Combat & Loadout
    [SerializeField] private GameObject victoryPanel;       // Combat victory new crops
    [SerializeField] private GameObject gameOverPanel;      // Dead UI
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject weaponChoicePanel;

    public event Action OnVictoryCompleted;

    private void Awake()
    {
        hudPanel?.SetActive(true);
    }

    // ----------------------
    // Show / Hide Methods
    // ----------------------
    /// <summary>
    /// Shows a screen. Pause can overlay HUD.
    /// </summary>
    public void Show(UIScreen screen)
    {
        switch (screen)
        {
            case UIScreen.HUD:
                hudPanel?.SetActive(true);
                break;
            case UIScreen.Victory:
                HideAllExcept(screen);
                victoryPanel?.SetActive(true);
                break;
            case UIScreen.GameOver:
                HideAllExcept(screen);
                gameOverPanel?.SetActive(true);
                break;
            case UIScreen.Pause:
                pausePanel?.SetActive(true); // overlay, do not hide HUD
                break;
        }
    }

    public void Hide(UIScreen screen)
    {
        switch (screen)
        {
            case UIScreen.HUD: hudPanel?.SetActive(false); 
                 break;

            case UIScreen.Victory: victoryPanel?.SetActive(false); 
                break;

            case UIScreen.GameOver: gameOverPanel?.SetActive(false); 
                break;

            case UIScreen.Pause: pausePanel?.SetActive(false); 
                break;
        }
    }

    /// <summary>
    /// Hides all panels except HUD and Pause overlay
    /// </summary>
    private void HideAllExcept(UIScreen exception)
    {
        if (exception != UIScreen.HUD) hudPanel?.SetActive(false);
        if (exception != UIScreen.Victory) victoryPanel?.SetActive(false);
        if (exception != UIScreen.GameOver) gameOverPanel?.SetActive(false);
        // pausePanel is never hidden here to allow overlay
    }

    /// <summary>
    /// Called when player clicks "Next" on Victory screen
    /// </summary>
    public void PlayerPressedVictoryNext()
    {
        victoryPanel.SetActive(false);  // Hide victory panel
        OnVictoryCompleted?.Invoke();// Fire event so GameManager knows
    }

    // Optional helpers for quick access
    public void ShowHUD() => Show(UIScreen.HUD);
    public void ShowVictory() => Show(UIScreen.Victory);
    public void ShowGameOver() => Show(UIScreen.GameOver);
    public void ShowPause() => Show(UIScreen.Pause);
}
