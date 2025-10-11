using UnityEngine;
using UnityEngine.SceneManagement;


// Current gameplay state
public enum GameState
{
    Playing,
    Paused,
    Victory,
    GameOver
}

// Current phase
public enum GamePhase
{
    Farm,
    Combat
}

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameInput gameInput;       // Player input
    [SerializeField] private UIManager uiManager;       // Control UI
    [SerializeField] private WaveManager waveManager;   // Manage enemy wave
    [SerializeField] private Player player;             // Reference to player
    [SerializeField] private FenceHealth fenceHealth;   // Reference to fence

    // Current gameplay state (default = playing)
    private GameState currentState = GameState.Playing;

    // Loaded save data (tracks current level + game phase)
    private SaveData saveData;

    private void OnEnable()
    {
        gameInput.OnPause += TogglePause;
    }

    private void OnDisable()
    {
        gameInput.OnPause -= TogglePause;
    }

    private void Awake()
    {
        // Load player progress from SaveSystem
        saveData = SaveSystem.LoadGame();

        // Safety check
        if (uiManager == null)
        {
            uiManager =FindFirstObjectByType<UIManager>();
        }
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    private void Start()
    {
        if (waveManager != null)
        {
            waveManager.OnLevelCompleted += HandleVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy += HandleGameOver;
        }

        // Start game
        Time.timeScale = 1f;
        uiManager.Show(UIScreen.HUD);
    }

    /// <summary>
    /// Called when all enemies in the level are defeated.
    /// </summary>
    private void HandleVictory()
    {
        if (currentState == GameState.Victory || currentState == GameState.GameOver)
        {
            return;
        }

        currentState=GameState.Victory;

        // Pause gameplay
        Time.timeScale = 0f;

        // Show victory UI
        uiManager.Show(UIScreen.Victory);
        Debug.Log("Victory! All enemies defeated.");

        // Increase and save progress
        saveData.currentLevel++;
        SaveSystem.SaveGame(saveData);

        // Save progress logic
        if (saveData.currentPhase == GamePhase.Combat)
        {
            saveData.currentLevel++;
            saveData.currentPhase = GamePhase.Farm;
        }
        else
        {
            saveData.currentPhase = GamePhase.Combat;
        }

        // Save progress 
        SaveSystem.SaveGame(saveData);
        Debug.Log($"Progress saved. Next level: {saveData.currentLevel}");
    }

    /// <summary>
    /// Called when the player loses (e.g. fence destroyed).
    /// </summary>
    private void HandleGameOver()
    {
        if (currentState == GameState.Victory || currentState == GameState.GameOver) 
        {
            return;
        }

        // Change state to gameover
        currentState = GameState.GameOver;

        // Pause time
        Time.timeScale = 0f;
        uiManager.Show(UIScreen.GameOver);
        Debug.Log("Game Over!");
    }

    /// <summary>
    /// Toggles between pause and resume when the player presses pause input.
    /// </summary>
    private void TogglePause()
    {
        if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (currentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Freezes time and shows the pause menu.
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0f;
        uiManager.Show(UIScreen.Pause);
        Debug.Log("Paused");
    }

    /// <summary>
    /// Unfreezes time and shows the HUD.
    /// </summary>
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        uiManager.Show(UIScreen.HUD);
        Debug.Log("Unpaused");
    }

    /// <summary>
    /// Reloads the current scene from the start.
    /// </summary>
    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Loads the next level or next phase (Farm > Combat or vice versa).
    /// </summary>
    private void LoadNextLevel()
    {
        Time.timeScale = 1f;

        string nextScene = "";

        // Decide scene name based on current phase
        if (saveData.currentPhase == GamePhase.Farm)
        {
            nextScene = $"Farm_{saveData.currentLevel}";
        }
        else
        {
            nextScene = $"Combat_{saveData.currentLevel}";
        }
        
        // Check if scene exists before trying to load
        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.Log($"Loading next scene{nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("Next level not found. Probably end game!");
            uiManager.Show(UIScreen.Victory);
        }
    }
}
