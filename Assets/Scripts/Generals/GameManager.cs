using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Player player;
    [SerializeField] private FenceHealth fenceHealth;

    private bool gameEnded = false;
    private bool isPaused = false;


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
        waveManager.OnLevelCompleted += HandleVictory;
        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy += HandleGameOver;
        }

        Time.timeScale = 1f;
        uiManager.Show(UIScreen.HUD);
    }



    private void HandleVictory()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        // Pause gameplay
        Time.timeScale = 0f;

        // Show victory UI
        uiManager.Show(UIScreen.Victory);
        Debug.Log("Victory! All enemies defeated.");
    }

    private void HandleGameOver()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        Time.timeScale = 0f;
        uiManager.Show(UIScreen.GameOver);
        Debug.Log("Game Over!");
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            uiManager.Show(UIScreen.HUD);
            Debug.Log("Unpaused");
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0f;
            uiManager.Show(UIScreen.Pause);
            Debug.Log("Paused");
        }
    }
}
