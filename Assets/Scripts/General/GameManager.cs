using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core manager that coordinates game state, saving, and scene transitions
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelDatabase levelDatabase;       // Assign the SO in inspector
    [SerializeField] private LevelManager levelManager;         // Level Manager
    [SerializeField] private WeaponInventory weaponInventory;   // Weapon Inventory
    [SerializeField] private AmmoInventory ammoInventory;       // Ammo Inventory
    [SerializeField] private UIManager UIManager;               // Control UI
    [SerializeField] private SceneController sceneController;   // Change scenes
    [SerializeField] private WaveManager waveManager;           // Manage enemy wave
    [SerializeField] private FenceHealth fenceHealth;           // Reference to fence
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private LevelRewardManager levelRewardManager;

    public SaveData SaveData { get; private set;}               // Loaded save data (tracks current level + game phase)

    private static GameManager instance;
    public static GameManager Instance => instance;             // Let other scripts access GameManager from anywhere

    // ------------------
    // LIFE CYCLE
    // ------------------
    private void Awake()
    {
        // Singleton pattern 
        if (instance != null && instance != this)
        {
            Debug.LogWarning("[GameManager] Duplicate instance detected — destroying new one.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Ensure GameManager is root before marking persistent
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // Load or create player progress from SaveSystem
        SaveData = SaveSystem.LoadGame();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Initialize all manager and inventories
        InitializeSystem();

        // Start correct phase based on GameState
        StartPhase(gameStateManager.CurrentPhase);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeEvent();
    }

    // ------------------
    // SCENE CHANGE LOGIC
    // ------------------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name}");

        // Rebind all scene-specific managers
        BindSceneReferences();

        // Initialize again (local scene references)
        InitializeSystem();

        // Start correct phase for new scene
        
    }

    /// <summary>
    /// Auto-find missing references
    /// </summary>
    private void BindSceneReferences()
    {
        UIManager ??= FindFirstObjectByType<UIManager>();
        waveManager ??= FindFirstObjectByType<WaveManager>();
        fenceHealth ??= FindFirstObjectByType<FenceHealth>();
        weaponInventory ??= FindFirstObjectByType<WeaponInventory>();
        ammoInventory ??= FindFirstObjectByType<AmmoInventory>();
        levelManager ??= FindFirstObjectByType<LevelManager>();
        sceneController ??= FindFirstObjectByType<SceneController>();
        gameStateManager ??= FindFirstObjectByType<GameStateManager>();
        levelRewardManager ??= FindFirstObjectByType<LevelRewardManager>();

        CheckReferences();
    }

    // ----------------------
    // SYSTEM INITIALIZATION
    // ----------------------
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

        // Events
        SubscribeEvent();

        Debug.Log("[Game Manager] [Inititialize] Systems initialized from SaveData.");
    }

    // ----------------------
    // GAME PHASE CONTROl
    // ----------------------
    private void StartPhase(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Farm:
                StartCombatPhase();
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
        //UIManager?.Show(UIScreen.Farm);
    }

    private void StartLoadoutPhase()
    {
        Debug.Log("[GameManager] Starting LOADOUT phase.");
        //UIManager?.Show(UIScreen.Loadout);
    }

    private void StartCombatPhase()
    {
        Debug.Log("[GameManager] Starting COMBAT phase.");

        // Initialize the current level from the database and start the game
        if (waveManager != null && levelDatabase != null)
        {
            LevelData currentLevelData = levelDatabase.GetLevelData(SaveData.currentLevel);
            waveManager.setLevel(currentLevelData);
            waveManager.BeginLevel();
        }

        UIManager?.Show(UIScreen.HUD);
        Time.timeScale = 1.0f;  
    }

    // ----------------------
    // GAME FLOW CONTROl
    // ----------------------
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
        UIManager.Show(UIScreen.GameOver);
        Debug.Log("[GameManager] Game Over!");
    }
    
    private string GetNextSceneName()
    {
        switch (gameStateManager.CurrentPhase)
        {
            case GamePhase.Farm:
                gameStateManager.SetGamePhase(GamePhase.Loadout);
                return "LoadoutScene";

            case GamePhase.Loadout:
                gameStateManager.SetGamePhase(GamePhase.Combat);
                return "CombatScene";

            case GamePhase.Combat:
                gameStateManager.SetGamePhase(GamePhase.Farm);
                return "FarmScene";

            default:
                return "FarmScene";
        }
    }

    // ----------------------
    // EVENT HANDLING
    // ----------------------

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
    }

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

    // ----------------------
    // SAFETY CHECK
    // ----------------------
    /// <summary>
    /// Check is any reference missing (human error)
    /// </summary>
    private void CheckReferences()
    {
        if (UIManager == null)
            Debug.LogError("[Game Manager] Missing reference to UIManager!");

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
}
