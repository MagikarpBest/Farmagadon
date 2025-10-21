using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core manager that coordinates game state, saving, and scene transitions
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private UIManager UIManager;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private FenceHealth fenceHealth;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private LevelRewardManager levelRewardManager;
    [SerializeField] private FarmController farmController;

    public SaveData SaveData { get; private set; }

    private void Awake()
    {
        // Load save data
        SaveData = SaveSystem.LoadGame();

        // Subscribe to scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        StartCoroutine(InitializeScene());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeEvents();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeScene());
    }

    private IEnumerator InitializeScene()
    {
        // Wait one frame to ensure all Awake() executed
        yield return null;

        // Auto-bind missing references if any
        UIManager ??= FindFirstObjectByType<UIManager>();
        waveManager ??= FindFirstObjectByType<WaveManager>();
        fenceHealth ??= FindFirstObjectByType<FenceHealth>();
        weaponInventory ??= FindFirstObjectByType<WeaponInventory>();
        ammoInventory ??= FindFirstObjectByType<AmmoInventory>();
        levelManager ??= FindFirstObjectByType<LevelManager>();
        sceneController ??= FindFirstObjectByType<SceneController>();
        gameStateManager ??= FindFirstObjectByType<GameStateManager>();
        levelRewardManager ??= FindFirstObjectByType<LevelRewardManager>();
        farmController ??= FindFirstObjectByType<FarmController>();

        // Initialize systems
        InitializeSystem();

        // Start the correct phase based on save data
        if (gameStateManager != null)
        {
            StartPhase(gameStateManager.CurrentPhase);
        }
        else
        {
            Debug.LogError("[GameManager] GameStateManager reference missing!");
        }
    }

    private void InitializeSystem()
    {
        weaponInventory?.InitializeFromSave(SaveData);
        ammoInventory?.InitializeFromSave(SaveData);
        levelManager?.InitializeFromSave(SaveData);

        SubscribeEvents();
    }

    // ----------------------
    // Game Phase
    // ----------------------
    private void StartPhase(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Farm: StartFarmPhase(); break;
            case GamePhase.Loadout: StartLoadoutPhase(); break;
            case GamePhase.Combat: StartCombatPhase(); break;
        }
    }

    private void StartFarmPhase()
    {
        UIManager?.Show(UIScreen.HUD);
        Debug.Log("[GameManager] FARM phase");

        waveManager?.BeginLevel(SaveData.currentLevel);
    }

    private void StartLoadoutPhase()
    {
        UIManager?.Show(UIScreen.HUD);
        Debug.Log("[GameManager] LOADOUT phase");

        waveManager?.BeginLevel(SaveData.currentLevel);
    }

    private void StartCombatPhase()
    {
        UIManager?.Show(UIScreen.HUD);
        Debug.Log("[GameManager] COMBAT phase");

        waveManager?.BeginLevel(SaveData.currentLevel);
        Time.timeScale = 1f;
    }

    // ----------------------
    // Event Handling
    // ----------------------
    private void SubscribeEvents()
    {
        if (waveManager != null) waveManager.OnLevelCompleted += HandleVictory;
        if (fenceHealth != null) fenceHealth.OnFenceDestroy += HandleGameOver;
        if (UIManager != null) UIManager.OnVictoryCompleted += OnVictoryUICompleted;
        if (levelRewardManager != null) levelRewardManager.OnRewardGiven += OnVictoryUICompleted;
        if (farmController != null) farmController.OnFarmEnd += OnFarmEnd;
    }

    private void UnsubscribeEvents()
    {
        if (waveManager != null) waveManager.OnLevelCompleted -= HandleVictory;
        if (fenceHealth != null) fenceHealth.OnFenceDestroy -= HandleGameOver;
        if (UIManager != null) UIManager.OnVictoryCompleted -= OnVictoryUICompleted;
        if (levelRewardManager != null) levelRewardManager.OnRewardGiven -= OnVictoryUICompleted;
        if (farmController != null) farmController.OnFarmEnd -= OnFarmEnd;
    }

    private void HandleVictory()
    {
        levelManager?.CompleteLevel();

        // Update save
        SaveData.currentLevel++;
        saveData.currentPhase = GamePhase.Loadout;
        SaveSystem.SaveGame(SaveData);

        sceneController?.LoadScene("LoadoutScene");
    }

    private void HandleGameOver()
    {
        Time.timeScale = 0f;
        UIManager?.Show(UIScreen.GameOver);
        Debug.Log("[GameManager] Game Over!");
    }

    private void OnVictoryUICompleted()
    {
        SaveAll();
        sceneController?.LoadScene(GetNextSceneName());
    }

    private void OnFarmEnd()
    {
        sceneController?.LoadScene(GetNextSceneName());
    }

    private string GetNextSceneName()
    {
        switch (gameStateManager.CurrentPhase)
        {
            case GamePhase.Farm: gameStateManager.SetGamePhase(GamePhase.Loadout); return "CombatScene";
            case GamePhase.Loadout: gameStateManager.SetGamePhase(GamePhase.Combat); return "CombatScene";
            case GamePhase.Combat: gameStateManager.SetGamePhase(GamePhase.Farm); return "FarmScene";
            default: return "FarmScene";
        }
    }

    private void SaveAll()
    {
        weaponInventory?.SaveToSaveData(SaveData);
        ammoInventory?.SaveToSaveData(SaveData);
        SaveSystem.SaveGame(SaveData);
    }
}