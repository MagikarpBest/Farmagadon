using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Game pausing function manager
/// </summary>
public class PauseManger : MonoBehaviour
{
    [SerializeField] private GameStateManager stateManager;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private UIManager UIManager;

    private void OnEnable()
    {
        if (gameInput != null)
        {
            //gameInput.OnPause += TogglePause;
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            //gameInput.OnPause -= TogglePause;
        }
    }

    /// <summary>
    /// Toggles between pause and resume when the player presses pause input
    /// </summary>
    private void TogglePause()
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
        UIManager.Show(UIScreen.Pause);
        Debug.Log("Paused");
    }

    /// <summary>
    /// Unfreezes time and shows the HUD.
    /// </summary>
    private void ResumeGame()
    {
        stateManager.SetGameState(GameState.Playing);
        Time.timeScale = 1f;
        UIManager.Hide(UIScreen.Pause);
        Debug.Log("Unpaused");
    }
}
