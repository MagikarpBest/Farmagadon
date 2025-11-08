using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Game pausing function manager
/// </summary>
public class PauseManger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameStateManager stateManager;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private UIManager UiManager;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause += TogglePause;
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause -= TogglePause;
        }
    }

    /// <summary>
    /// Toggles between pause and resume when the player presses pause input
    /// </summary>
    public void TogglePause()
    {
        if (stateManager.CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (stateManager.CurrentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Freezes time and shows the pause menu.
    /// </summary>
    private void PauseGame()
    {
        stateManager.SetGameState(GameState.Paused);
        Time.timeScale = 0f;
        UiManager.ShowVictory();
        Debug.Log("Paused");
    }

    /// <summary>
    /// Unfreezes time and shows the HUD.
    /// </summary>
    private void ResumeGame()
    {
        stateManager.SetGameState(GameState.Playing);
        Time.timeScale = 1f;
        //UIManager.HidePause(false);
        UiManager.HideVictory();
        Debug.Log("Unpaused");
    }
}
