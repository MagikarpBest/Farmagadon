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
    public delegate void CropFarmed();
    public CropFarmed OnCropFarmed;
    public delegate void SetRecommended(ENEMY_WEAKNESS[] placeholderParam);
    public SetRecommended OnGetRecommended;

    

    private void Awake()
    {
        saveData = SaveSystem.LoadGame(); // this one i just copy wat i saw from the game manager script
    }
    
    public void cropFarmed(AmmoData cropName, int dropAmount)
    {
        ammoInventory.AddAmmo(cropName, dropAmount); // add to ammo inv is here
        OnCropFarmed?.Invoke(); // this one connects to BulletPanelHandler, just to update the UI
    }

    public void Start()
    {
        gameStart?.Invoke(); // ideally this should start the whole farm sequence+UI but i not sure how exactly it will happen so for now it runs on start
        //ENEMY_WEAKNESS[] testList = { ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice}; these 2 ignore for now. havent finished
        //OnGetRecommended?.Invoke(testList);
    }
    
    public void SavePlayerData() // this one also copy from the gamemanager script
    {
        ammoInventory.SaveToSaveData(saveData);
        weaponInventory.SaveToSaveData(saveData);
        SaveSystem.SaveGame(saveData);
    }
}
