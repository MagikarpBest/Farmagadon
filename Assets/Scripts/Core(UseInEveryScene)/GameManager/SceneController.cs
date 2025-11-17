 using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameStateManager gameStateManager;

    private GamePhase nextPhase;

    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// load next scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        Debug.Log($"[SceneController] Loading next scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
