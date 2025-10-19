using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private UIManager uimanager;
    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Loads the next level or next phase (Farm > Combat or vice versa).
    /// </summary>
    public void LoadNextLevel(int nextLevel, GamePhase phase)
    {
        Time.timeScale = 1f;

        // Decide scene name based on current phase
        string nextScene = phase == GamePhase.Farm
                                 ? $"Farm_{nextLevel}"
                                 : $"Combat_{nextLevel}";

        uimanager.Show(UIScreen.Victory);
        // Check if scene exists before trying to load
        /*if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.Log($"Loading next scene{nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("Next level not found. Probably end game!");

        }*/
    }
}
