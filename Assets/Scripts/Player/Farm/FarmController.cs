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


    public event Action StartFarmCycle;
    public event Action StopFarmCycle;
    public event Action OnCropFarmed;

    //public delegate void StartGame();
    //public StartGame gameStart;
    //public delegate void EndGame();
    //public EndGame gameEnd; // invoked from Timer.cs
    //public delegate void CropFarmed();
    //public CropFarmed OnCropFarmed; // invoked from plants.cs -> GridController.cs

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
        StartFarmCycle?.Invoke();
    }


    public void cropFarmed(AmmoData cropName, int dropAmount)
    {
        ammoInventory.AddAmmo(cropName, dropAmount); // add to ammo inv is here
        OnCropFarmed?.Invoke(); // this one connects to BulletPanelHandler, just to update the UI
    }

    public void BeginFarmCycle(int level)
    {
        DayCycleLevelData levelData = dayCycleLevelManager.GetLevelData(level);
        if (levelData == null) 
        {
            Debug.LogWarning("THERES NO LEVEL BRUH");
            return; 
        }

    }
    public void EndFarmCycle()
    {
        stopGame = true;
        StopFarmCycle?.Invoke();
    }

    public void InitializeFromSave(SaveData data)
    {
        // store reference to SaveData if you want
        // or just call your inventory init methods
        if (ammoInventory != null)
            ammoInventory.InitializeFromSave(data); // should populate ammo from SaveData. See note below.

        if (weaponInventory != null)
            weaponInventory.InitializeFromSave(data);

        Debug.Log("[FarmController] Initialized from SaveData.");
    }

    // When farm ends, copy runtime inventory state back to SaveData
    public void ApplyProgressToSave(SaveData data)
    {
        if (ammoInventory != null)
            ammoInventory.SaveToSaveData(data); // writes lists into SaveData. See note below.

        if (weaponInventory != null)
            weaponInventory.SaveToSaveData(data);

        Debug.Log("[FarmController] Applied farm progress to SaveData.");
    }
}