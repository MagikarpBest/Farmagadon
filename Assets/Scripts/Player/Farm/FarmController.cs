using UnityEngine;
using Farm;
public class FarmController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridController gridController;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Timer farmTimer;
    public AmmoInventory AmmoInventory { get { return ammoInventory; } }

    private SaveData saveData;
    private bool stopGame = false; 
    public bool StopGame { get { return stopGame; } set { stopGame = value; } }

    public delegate void StartGame();
    public StartGame OnFarmStart;
    public delegate void EndGame();
    public EndGame OnFarmEnd; // invoked from Timer.cs
    public delegate void CropFarmed();
    public CropFarmed OnCropFarmed; // invoked from plants.cs -> GridController.cs
    public delegate void SetRecommended(ENEMY_WEAKNESS[] placeholderParam); // ignore for now
    public SetRecommended OnGetRecommended;


    // ----------------------
    // Initialization
    // ----------------------
    public void InitializeFromSave(SaveData data)
    {
        saveData = data;

        ammoInventory?.InitializeFromSave(saveData);
        weaponInventory?.InitializeFromSave(saveData);

        Debug.Log("[FarmController] Initialized with SaveData.");
    }
    
    public void cropFarmed(AmmoData cropName, int dropAmount)
    {
        ammoInventory.AddAmmo(cropName, dropAmount); // add to ammo inv is here
        OnCropFarmed?.Invoke(); // this one connects to BulletPanelHandler, just to update the UI
    }

    public void Start()
    {
        OnFarmStart?.Invoke(); // ideally this should start the whole farm sequence+UI but i not sure how exactly it will happen so for now it runs on start
        //ENEMY_WEAKNESS[] testList = { ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice}; these 2 ignore for now. havent converted to SO yet
        //OnGetRecommended?.Invoke(testList);
    }
    
    public void SavePlayerData() // this one also copy from the gamemanager script
    {
        ammoInventory.SaveToSaveData(saveData);
        weaponInventory.SaveToSaveData(saveData);
        SaveSystem.SaveGame(saveData);
    }
}
