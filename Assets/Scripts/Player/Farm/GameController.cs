using UnityEngine;
using System.Collections.Generic;
using Farm;
public class GameController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    public AmmoInventory AmmoInventory { get { return ammoInventory; } }
    
    private SaveData saveData;

    public delegate void StartGame();
    public StartGame gameStart;
    public delegate void CropFarmed(AmmoData cropName);
    public CropFarmed OnCropFarmed;
    public delegate void SetRecommended(ENEMY_WEAKNESS[] placeholderParam);
    public SetRecommended OnGetRecommended;

    

    private void Awake()
    {
        saveData = SaveSystem.LoadGame();
    }
    
    public void cropFarmed(AmmoData cropName, int dropAmount)
    {
        print(cropName);
        print(dropAmount);
        ammoInventory.AddAmmo(cropName, dropAmount);
        OnCropFarmed?.Invoke(cropName);
    }
    public void Start()
    {
        gameStart?.Invoke();
        ENEMY_WEAKNESS[] testList = { ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice};
        OnGetRecommended?.Invoke(testList);
    }
    
    public void SavePlayerData()
    {
        ammoInventory.SaveToSaveData(saveData);
        weaponInventory.SaveToSaveData(saveData);
        SaveSystem.SaveGame(saveData);
    }
}
