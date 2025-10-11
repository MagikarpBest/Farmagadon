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
        if (waveManager != null)
        {
            waveManager.OnLevelCompleted += HandleVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy += HandleGameOver;
        }
        gameInput.OnPause += TogglePause;
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnLevelCompleted -= HandleVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy -= HandleGameOver;
        }
        gameInput.OnPause -= TogglePause;
    }

    private void Awake()
    {
        // Load player progress from SaveSystem
        saveData = SaveSystem.LoadGame();

        // Safety check
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        if (fenceHealth == null)
        {
            fenceHealth = FindFirstObjectByType<FenceHealth>();
        }
        if (gameInput == null)
        {
            gameInput = FindFirstObjectByType<GameInput>();
        }
        CheckReferences();
    }

    private void Start()
    {
        if (waveManager != null)
        {
            waveManager.LoadLevelData(saveData.currentLevel);
        }
        // Start game
        Time.timeScale = 1f;
        uiManager.Show(UIScreen.HUD);
    }

    private void CheckReferences()
    {
        if (uiManager == null)
        {
            Debug.LogError(" GameManager: Missing reference to UIManager!");
        }

        if (waveManager == null)
        {
            Debug.LogWarning(" GameManager: WaveManager not found — waves won't trigger victory.");
        }

        if (fenceHealth == null)
        {
            Debug.LogWarning(" GameManager: FenceHealth not found — GameOver won't trigger.");
        }

        if (player == null)
        {
            Debug.LogWarning(" GameManager: Player reference missing!");
        }

        if (gameInput == null)
        {
            Debug.LogWarning(" GameManager: GameInput reference missing — pause won't work.");
        }
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

        currentState = GameState.Victory;

        // Pause gameplay
        Time.timeScale = 0f;

        // Show victory UI
        uiManager.Show(UIScreen.Victory);
        Debug.Log("Victory! All enemies defeated.");

        if (saveData == null)
        {
            Debug.LogWarning("SaveData is null — creating new.");
            saveData = new SaveData();
        }

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
        Debug.Log($"Progress saved. Next level: {saveData.currentLevel}, Next phase: {saveData.currentPhase}");
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
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        uiManager.Show(UIScreen.Pause);
        Debug.Log("Paused");
    }

    /// <summary>
    /// Unfreezes time and shows the HUD.
    /// </summary>
    private void ResumeGame()
    {
        currentState = GameState.Playing;
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
        if (saveData == null)
        {
            Debug.LogError("SaveData missing cannot load next level!");
            return;
        }

        Time.timeScale = 1f;

        // Decide scene name based on current phase
        string nextScene;

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
