using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core manager that coordinates game state, saving, and scene transitions
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelDatabase levelDatabase;        // Assign the SO in inspector
    [SerializeField] private LevelManager levelManager;         // Level Manager
    [SerializeField] private WeaponInventory weaponInventory;   // Weapon Inventory
    [SerializeField] private AmmoInventory ammoInventory;       // Ammo Inventory
    [SerializeField] private UIManager UIManager;               // Control UI
    [SerializeField] private SceneController sceneController;
    [SerializeField] private WaveManager waveManager;           // Manage enemy wave
    [SerializeField] private FenceHealth fenceHealth;           // Reference to fence


    public SaveData SaveData { get; private set;}               // Loaded save data (tracks current level + game phase)
    
    // ----------------------
    // EVENT SUBSCRIPTION
    // ----------------------
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
    }

    // ----------------------
    // EVENT SUBSCRIPTION
    // ----------------------
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
    }

    // ------------------
    // INITIALIZATION
    // ------------------
    private void Awake()
    {
        // Auto-find missing references
        UIManager ??=FindFirstObjectByType<UIManager>();
        waveManager ??= FindFirstObjectByType<WaveManager>();
        fenceHealth ??= FindFirstObjectByType<FenceHealth>();
        weaponInventory ??= FindFirstObjectByType<WeaponInventory>();
        ammoInventory ??= FindFirstObjectByType<AmmoInventory>();

        CheckReferences();
    }
    // ----------------------
    // SAFETY CHECK
    // ----------------------
    private void CheckReferences()
    {
        if (UIManager == null)
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

        if (weaponInventory == null)
        {
            Debug.LogWarning(" Weapon inventory reference missing");
        }
        if (ammoInventory == null)
        {
            Debug.LogWarning(" Ammo inventory reference missing");
        }
    }

    private void Start()
    {
        // Safety check
        if (weaponInventory == null || ammoInventory == null || levelManager == null || waveManager == null || levelDatabase == null || UIManager == null)
        {
            Debug.LogError("GameManager missing references!");
            return;
        }
        // Load player progress from SaveSystem
        SaveData = SaveSystem.LoadGame();
        if (SaveData == null)
        {
            SaveData = new SaveData(); // fallback for first time player
        }
        // Initialize inventories
        weaponInventory.InitializeFromSave(SaveData);
        ammoInventory.InitializeFromSave(SaveData);

        // Initialize levelManager
        levelManager.InitializeFromSave(SaveData);

        // Initialize the current level from the database
        if (waveManager != null && levelDatabase != null) 
        {
            LevelData currentLevelData = levelDatabase.GetLevelData(SaveData.currentLevel);
            waveManager.setLevel(currentLevelData);
        }
        // Start game
        waveManager.BeginLevel();
        Time.timeScale = 1f;
        UIManager.Show(UIScreen.HUD);
    }

    

    // ----------------------
    // GAME STATE LOGIC
    // ----------------------

    /// <summary>
    /// Called when all enemies in the level are defeated.
    /// </summary>
    private void HandleVictory()
    {
        // Give rewards
        levelManager.CompleteLevel(); 

        // Save progress
        saveAll();

        // Load next scene
        sceneController?.LoadScene("CombatScene");

        Debug.Log($"Progress saved. Next level: {SaveData.currentLevel}, Next phase: {SaveData.currentPhase}");
    }

    /// <summary>
    /// Called when the player loses (e.g. fence destroyed).
    /// </summary>
    private void HandleGameOver()
    {
        // Pause gameplay
        Time.timeScale = 0f;
        UIManager.Show(UIScreen.GameOver);
        Debug.Log("Game Over!");
    }

    

    // ----------------------
    // SAVE
    // ----------------------
    private void saveAll()
    {
        weaponInventory.SaveToSaveData(SaveData);
        ammoInventory.SaveToSaveData(SaveData);
        SaveSystem.SaveGame(SaveData);
    }

    
}
