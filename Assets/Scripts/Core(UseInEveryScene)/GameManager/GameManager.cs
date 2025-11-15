using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;

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
    [SerializeField] private CircleTransition circleTransition;
    [SerializeField] private SceneController sceneController;       // Change scenes
    [SerializeField] private WaveManager waveManager;               // Manage enemy wave
    [SerializeField] private FenceHealth fenceHealth;               // Reference to fence
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private LevelRewardManager levelRewardManager;
    [SerializeField] private FarmController farmController;

    [Header("BGMs")]
    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip farmBGM;
    [SerializeField] private AudioClip loadoutBGM;
    [SerializeField] private AudioClip combatBGM;
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
        UnityAudioManager unityAudioManagerPrefab = Resources.Load<UnityAudioManager>("UnityAudioManager");
        UnityAudioManager unityAudioManagerInstance = GameObject.Instantiate(unityAudioManagerPrefab);
        unityAudioManagerInstance.Initiallize();
        unityAudioManagerInstance.name = "AudioManager";
        AudioService.SetAudioManager(unityAudioManagerInstance);
        Debug.Log($"current phase = {SaveData.currentPhase} ");
        AudioHandling(gameStateManager.CurrentPhase);
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
                StartCoroutine(StartFarmPhase());
                break;

            case GamePhase.Loadout:
                StartCoroutine(StartLoadoutPhase());
                break;

            case GamePhase.Combat:
                StartCoroutine(StartCombatPhase());
                break;
        }
    }

    private IEnumerator StartFarmPhase()
    {
        yield return null;
        Debug.Log("[GameManager] Starting FARM phase.");
        UIManager?.ShowHUD();
        Time.timeScale = 0.0f;
        //yield return circleTransition.CloseTransition();


        // Initialize the current level from the database and start the game
        farmController.BeginFarmCycle(SaveData.currentLevel - 1);
        Debug.Log($"Starting farm level");
        Time.timeScale = 1.0f;
    }

    private IEnumerator StartLoadoutPhase()
    {
        yield return null;
        Debug.Log("[GameManager] Starting LOADOUT phase.");
        UIManager?.ShowHUD();
        Time.timeScale = 0.0f;
        //yield return circleTransition.CloseTransition();

        // Just debug, remove in ltr
        // Initialize the current level from the database and start the game
        waveManager?.BeginLevel(SaveData.currentLevel);
        Debug.Log($"Starting loadout level");
        Time.timeScale = 1.0f;
    }

    private IEnumerator StartCombatPhase()
    {
        yield return null;
        Debug.Log("[GameManager] Starting COMBAT phase.");
        UIManager?.ShowHUD();
        Time.timeScale = 0.0f;
        //yield return circleTransition.CloseTransition();
        // Initialize the current level from the database and start the game

        waveManager?.BeginLevel(SaveData.currentLevel);
        Debug.Log($"spawning level");
        Time.timeScale = 1.0f;
    }
    #endregion

    #region Audio Handling
    private void AudioHandling(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Farm:
                AudioService.AudioManager.PlayBGM(farmBGM, 1f);
                break;

            case GamePhase.Loadout:
                AudioService.AudioManager.PlayBGM(loadoutBGM, 1f);
                break;

            case GamePhase.Combat:
                AudioService.AudioManager.PlayBGM(combatBGM,1f);
                break;
        }
    }

    #endregion

    #region Game Flow Control
    private void OnFarmEnd()
    {
        SaveAll();
        Debug.Log("[GameManager] Farm level completed!");
        sceneController.LoadScene(GetNextSceneName());
    }
    /// <summary>
    /// Called when all enemies in the level are defeated.
    /// </summary>
    private void OnCombatVictory()
    {
        // i have to do this or else i have to refractor alot of event linking which is smtg im lazy to do
        StartCoroutine(HandleCombatVictory());
    }
    private IEnumerator HandleCombatVictory()
    {
        // Complete level (give rewards, update progression)
        Debug.Log("[GameManager] Level completed!");
        yield return circleTransition.OpenTransition();
        levelManager.CompleteLevel();
        yield return circleTransition.CloseTransition();
    }

    /// <summary>
    /// Called when player selected reward or press next in combat.
    /// </summary>
    private void OnVictoryUICompleted()
    {
        // i have to do this or else i have to refractor alot of event linking which is smtg im lazy to do
        StartCoroutine(HandleVictoryUIComplete());
    }

    private IEnumerator HandleVictoryUIComplete()
    {
        SaveAll();
        Debug.Log($"[TEST] CurrentPhase before load: {gameStateManager.CurrentPhase}");
        Debug.Log($"[GameManager] Progress saved. Next level: {SaveData.currentLevel + 1}, Next phase: {gameStateManager.CurrentPhase}");
        yield return circleTransition.OpenTransition();
        sceneController.LoadScene(GetNextSceneName());
    }

    /// <summary>
    /// Called when the player loses (e.g. fence destroyed).
    /// </summary>
    private void OnCombatGameOver()
    {
        // Pause gameplay
        Time.timeScale = 0f;
        UIManager.ShowGameOver();
        Debug.Log("[GameManager] Game Over!");
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
            waveManager.OnLevelCompleted += OnCombatVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy += OnCombatGameOver;
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
            waveManager.OnLevelCompleted -= OnCombatVictory;
        }

        if (fenceHealth != null)
        {
            fenceHealth.OnFenceDestroy -= OnCombatGameOver;
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

        if (farmController == null)
            Debug.LogWarning("[Game Manager] FarmController reference missing! Farm level won't trigger properly.");
    }
    #endregion
}
