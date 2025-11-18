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
    Settings,
    FarmTutorial
}

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject hudPanel;           // Main HUD for both Farm & Combat & Loadout
    [SerializeField] private GameObject victoryPanel;       // Combat victory new crops
    [SerializeField] private GameObject gameOverPanel;      // Dead UI
    [SerializeField] private GameObject pausePanel;         // Pause UI
    [SerializeField] private GameObject settingsPanel;       // Pause setting UI
    [SerializeField] private GameObject farmTutorialPanel;      // Tutorial UI


    public event Action OnVictoryCompleted;

    private void Awake()
    {
        if (hudPanel != null)
        {
            ShowHUD();
        }
    }

    #region Show/Hide
    // ----------------------
    // Show / Hide Methods
    // ----------------------
    /// <summary>
    /// Shows a screen. Pause can overlay HUD.
    /// </summary>
    private void Show(UIScreen screen)
    {
        switch (screen)
        {
            case UIScreen.HUD:
                SafeShow(hudPanel);
                break;

            case UIScreen.Victory:
                SafeShow(victoryPanel);
                HideAllExcept(screen);
                break;

            case UIScreen.GameOver:
                HideAllExcept(screen);
                SafeShow(gameOverPanel);
                break;

            case UIScreen.Pause:
                SafeShow(pausePanel);
                break;

            case UIScreen.Settings:
                SafeHide(pausePanel, true);
                SafeShow(settingsPanel);
                break;

            case UIScreen.FarmTutorial:
                SafeShow(farmTutorialPanel);
                break;
        }
    }

    private void Hide(UIScreen screen, bool rememberNavigation = false)
    {
        switch (screen)
        {
            case UIScreen.HUD:
                SafeHide(hudPanel);
                break;

            case UIScreen.Victory:
                SafeHide(victoryPanel);
                break;

            case UIScreen.GameOver:
                SafeHide(gameOverPanel);
                break;

            case UIScreen.Pause:
                SafeHide(pausePanel);
                break;

            case UIScreen.Settings:
                SafeHide(settingsPanel);
                ShowPause(true);
                break;
            case UIScreen.FarmTutorial:
                SafeHide(farmTutorialPanel);
                break;
        }
    }

    // Navigations helpers
    /// <summary>
    /// Start first selected navigation and save last selected memory
    /// </summary>
    private void SafeShow(GameObject panel, bool resetMemory = false)
    {
        if (panel == null)
        {
            return;
        }

        var navigation = panel.GetComponent<UINavigationMemory>();
        if (resetMemory)
        {
            navigation?.ResetMemory();
        }

        panel.SetActive(true);
        navigation?.ActivateUI();
    }

    /// <summary>
    ///  If memory remember is ticked, when menu close it will remember.
    /// </summary>
    private void SafeHide(GameObject panel, bool remember = false)
    {
        if (panel == null) 
        { 
            return; 
        }

        var navigation = panel.GetComponent<UINavigationMemory>();
        if (navigation != null)
        {
            if (remember)
            {
                navigation.DeactivateUI();
            }
            else 
            {
                navigation.ResetMemory();
            }
        }
        panel.SetActive(false);
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

    #endregion
    // ===========================
    // HELPERS / EVENTS
    // ===========================
    /// <summary>
    /// Called when player clicks "Next" on Victory screen
    /// </summary>
    public void PlayerPressedVictoryNext()
    {
        //victoryPanel.SetActive(false);  // Hide victory panel
        OnVictoryCompleted?.Invoke();// Fire event so GameManager knows
    }

    // Optional helpers for quick access
    public void ShowHUD() => Show(UIScreen.HUD);
    public void ShowVictory() => Show(UIScreen.Victory);
    public void ShowGameOver() => Show(UIScreen.GameOver);
    public void ShowPause(bool rememberNavigation = false) => Show(UIScreen.Pause);
    public void ShowSettings() => Show(UIScreen.Settings);
    public void ShowFarmTutorial() => Show(UIScreen.FarmTutorial);
    public void HideSettings() => Hide(UIScreen.Settings);
    public void HideVictory() => Hide(UIScreen.Victory);
    public void HidePause(bool rememberNavigation = false) => Hide(UIScreen.Pause, rememberNavigation);
    public void HideFarmTutorial() => Hide(UIScreen.FarmTutorial);
}
