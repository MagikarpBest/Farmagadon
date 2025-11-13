using UnityEngine;
using System.Collections.Generic;
using Farm;
using System;
public class FarmController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private DayCycleLevelManager dayCycleLevelManager;
    [SerializeField] private FarmTimer timer;
    public AmmoInventory AmmoInventory { get { return ammoInventory; } }
    public WeaponInventory WeaponInventory { get { return weaponInventory; } }

    private bool stopGame = false;
    public bool StopGame { get { return stopGame; } }

    private SaveData saveData;

    public event Action StartFarmCycle;
    public event Action StopFarmCycle;
    public event Action OnCropFarmed;

    public delegate void FarmStarted(DayCycleLevelData data);
    public FarmStarted StartGridPlanting;
    public FarmStarted SetUpcomingEnemies;
    

    public void OnEnable()
    {
        timer.OnTimerEnded += EndFarmCycle;
        
    }

    public void OnDisable()
    {
        timer.OnTimerEnded -= EndFarmCycle;
    }

    public void Start()
    {
        //gameStart?.Invoke(); // ideally this should start the whole farm sequence+UI but i not sure how exactly it will happen so for now it runs on start
        saveData = SaveSystem.LoadGame();
        StartFarmCycle?.Invoke();
        BeginFarmCycle(saveData.currentLevel-1);
    }


    public void CropFarmed(AmmoData cropName, int dropAmount)
    {
        ammoInventory.AddAmmo(cropName, dropAmount); // add to ammo inv is here
        //OnCropFarmed?.Invoke(); // this one connects to BulletPanelHandler, just to update the UI
    }

    public void BeginFarmCycle(int level)
    {
        DayCycleLevelData levelData = dayCycleLevelManager.GetLevelData(level);
        if (levelData == null) 
        {
            Debug.LogWarning("THERES NO LEVEL BRUH");
            return; 
        }
        StartGridPlanting?.Invoke(levelData);
        SetUpcomingEnemies?.Invoke(levelData);
    }
    public void EndFarmCycle()
    {
        stopGame = true;
        StopFarmCycle?.Invoke();
    }

}