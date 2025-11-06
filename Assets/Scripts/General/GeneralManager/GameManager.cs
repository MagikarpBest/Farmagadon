using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core manager that coordinates game state, saving, and scene transitions.
/// Each scene has its own instance.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelManager levelManager;             // Level Manager
    [SerializeField] private WeaponInventory weaponInventory;       // Weapon Inventory
    [SerializeField] private AmmoInventory ammoInventory;           // Ammo Inventory
    [SerializeField] private UIManager UIManager;                   // Control UI
    [SerializeField] private SceneController sceneController;       // Change scenes
    [SerializeField] private WaveManager waveManager;               // Manage enemy wave
    [SerializeField] private FenceHealth fenceHealth;               // Reference to fence
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private LevelRewardManager levelRewardManager;
    [SerializeField] private FarmController farmController;
    [SerializeField] private FarmDataBridge farmDataBridge;

    public SaveData SaveData { get; private set;}               // Loaded save data (tracks current level + game phase)

    #region Life Cycle
    private void Awake()
    {
        // Load or create player progress from SaveSystem
        SaveData = SaveSystem.LoadGame();
    }

    private void Start()
    {
        // Initialize all manager and inventories
        InitializeSystem();
        CheckReferences();
        StartPhase(gameStateManager.CurrentPhase);
        Debug.Log($"current phase = {SaveData.currentPhase} ");
    }

    private void OnDisable()
    {
        UnsubscribeEvent();
    }
    #endregion

    #region System Initialization
    private void InitializeSystem()
    {
        if (weaponInventory != null && SaveData != null)
        {
            weaponInventory.InitializeFromSave(SaveData);
        }

        if (ammoInventory != null && SaveData != null)
        {
            ammoInventory.InitializeFromSave(SaveData);
        }

        if (levelManager != null && SaveData != null)
        {
            levelManager.InitializeFromSave(SaveData);
        }

        if (gameStateManager != null && SaveData != null)
        {
            gameStateManager.SetGamePhase(SaveData.currentPhase);
        }
        // Events
        SubscribeEvent();

        Debug.Log("[Game Manager] [Inititialize] Systems initialized from SaveData.");
    }
    #endregion

    #region Game Phase Control
    private void StartPhase(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Farm:
                StartFarmPhase();
                break;

            case GamePhase.Loadout:
                StartLoadoutPhase();
                break;

            case GamePhase.Combat:
                StartCombatPhase();
                break;
        }
    }

    private void StartFarmPhase()
    {
        Debug.Log("[GameManager] Starting FARM phase.");
        if (farmDataBridge != null)
        {
            farmDataBridge.Initialize(SaveData);
        }
        UIManager?.ShowHUD();
        Time.timeScale = 1.0f;

        // Initialize the current level from the database and start the game
        waveManager?.BeginLevel(SaveData.currentLevel);
        Debug.Log($"spawning level");
    }

    private void StartLoadoutPhase()
    {
        Debug.Log("[GameManager] Starting LOADOUT phase.");
        UIManager?.ShowHUD();
        Time.timeScale = 1.0f;

        // Initialize the current level from the database and start the game
        waveManager?.BeginLevel(SaveData.currentLevel);
        Debug.Log($"spawning level");
    }

    private void StartCombatPhase()
    {
        Debug.Log("[GameManager] Starting COMBAT phase.");

        // Initialize the current level from the database and start the game
        waveManager?.BeginLevel(SaveData.currentLevel);
        Debug.Log($"spawning level");
        Time.timeScale = 1.0f;
    }
    #endregion

    #region Game Flow Control
    /// <summary>
    /// Called when all enemies in the level are defeated.
    /// </summary>
    private void HandleVictory()
    {
        // Complete level (give rewards, update progression)
        Debug.Log("[GameManager] Level completed!");
        levelManager.CompleteLevel();
    }

    /// <summary>
    /// Called when player selected reward or press next in combat.
    /// </summary>
    public void OnVictoryUICompleted()
    {
        SaveAll();
        Debug.Log($"[TEST] CurrentPhase before load: {gameStateManager.CurrentPhase}");
        Debug.Log($"[GameManager] Progress saved. Next level: {SaveData.currentLevel + 1}, Next phase: {gameStateManager.CurrentPhase}");
        sceneController.LoadScene(GetNextSceneName());
    }

    /// <summary>
    /// Called when the player loses (e.g. fence destroyed).
    /// </summary>
    private void HandleGameOver()
    {
        // Pause gameplay
        Time.timeScale = 0f;
        UIManager.ShowGameOver();
        Debug.Log("[GameManager] Game Over!");
    }

    /// <summary>
    /// Called when farm end to switch scene.
    /// </summary>
    private void OnFarmEnd()
    {
        farmDataBridge.SaveProgress();
        sceneController.LoadScene(GetNextSceneName());
    }

    /// <summary>
    /// Use to get next scene name and change scene/
    /// </summary>
    private string GetNextSceneName()
    {
        switch (gameStateManager.CurrentPhase)
        {
            case GamePhase.Farm:
                SaveData.currentPhase = GamePhase.Loadout;
                gameStateManager.SetGamePhase(GamePhase.Loadout);
                SaveSystem.SaveGame(SaveData);
                return "LoadOut";

            case GamePhase.Loadout:
                SaveData.currentPhase = GamePhase.Combat;
                gameStateManager.SetGamePhase(GamePhase.Combat);
                SaveSystem.SaveGame(SaveData);
                return "CombatScene";

            case GamePhase.Combat:
                SaveData.currentPhase = GamePhase.Farm;
                gameStateManager.SetGamePhase(GamePhase.Farm);
                SaveSystem.SaveGame(SaveData);
                return "FarmScene";

            default:
                SaveData.currentPhase = GamePhase.Farm;
                SaveSystem.SaveGame(SaveData);
                return "FarmScene";
        }
    }
    #endregion

    #region Event Handling
    private void SubscribeEvent()
    {
        if (waveManager != null)
        {
            waveManager.OnLevelCompleted += HandleVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy += HandleGameOver;
        }

        if (UIManager != null)
        {
            UIManager.OnVictoryCompleted += OnVictoryUICompleted;
        }

        if (levelRewardManager != null)
        {
            levelRewardManager.OnRewardGiven += OnVictoryUICompleted;
        }

        if (farmController != null)
        {
            farmController.StopFarmCycle += OnFarmEnd;
        }
    }

    private void UnsubscribeEvent()
    {
        if (waveManager != null)
        {
            waveManager.OnLevelCompleted -= HandleVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy -= HandleGameOver;
        }

        if (UIManager != null)
        {
            UIManager.OnVictoryCompleted -= OnVictoryUICompleted;
        }

        if (levelRewardManager != null)
        {
            levelRewardManager.OnRewardGiven -= OnVictoryUICompleted;
        }

        if (farmController != null)
        {
            farmController.StopFarmCycle -= OnFarmEnd;
        }
    }
    #endregion

    #region Save System
    // ----------------------
    // SAVE SYSTEM
    // ----------------------
    private void SaveAll()
    {
        weaponInventory.SaveToSaveData(SaveData);
        ammoInventory.SaveToSaveData(SaveData);
        SaveSystem.SaveGame(SaveData);
        Debug.Log(" Game saved after reward!");
    }
    #endregion

    #region Safety Check
    // ----------------------
    // SAFETY CHECK
    // ----------------------
    /// <summary>
    /// Check is any reference missing (human error)
    /// </summary>
    private void CheckReferences()
    {
        if (UIManager == null)
            Debug.LogWarning("[Game Manager] Missing reference to UIManager!");

        if (waveManager == null)
            Debug.LogWarning("[Game Manager] WaveManager not found — waves won't trigger victory.");

        if (fenceHealth == null)
            Debug.LogWarning("[Game Manager] FenceHealth not found — GameOver won't trigger.");

        if (weaponInventory == null)
            Debug.LogWarning("[Game Manager] WeaponInventory reference missing.");

        if (ammoInventory == null)
            Debug.LogWarning("[Game Manager] AmmoInventory reference missing.");

        if (levelManager == null)
            Debug.LogWarning("[Game Manager] LevelManager reference missing.");

        if (sceneController == null)
            Debug.LogWarning("[Game Manager] SceneController reference missing.");

        if (gameStateManager == null)
            Debug.LogWarning($"[Game Manager] GameStateManager reference missing.");

        if (levelRewardManager == null)
            Debug.LogWarning("[Game Manager] LevelRewardManager reference missing! Rewards won't trigger properly.");
    }
    #endregion
}
