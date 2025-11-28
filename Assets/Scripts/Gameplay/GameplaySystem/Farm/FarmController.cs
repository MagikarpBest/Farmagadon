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
    [SerializeField] private AudioClip debugMusic;
    public AmmoInventory AmmoInventory { get { return ammoInventory; } }
    public WeaponInventory WeaponInventory { get { return weaponInventory; } }
    private static bool cycleStarted = false;
    public static bool GetCycleStarted => cycleStarted;

    private bool stopGame = false;
    public bool StopGame { get { return stopGame; } }

    public static SaveData saveData;

    public event Action StartFarmCycle;
    public event Action StopFarmCycle;
    public event Action OnCropFarmed;

    public delegate void FarmStarted(DayCycleLevelData data);
    public FarmStarted StartGridPlanting;
    public FarmStarted SetUpcomingEnemies;
    

    public void OnEnable()
    {
        timer.OnTimerEnded += EndFarmCycle;
        Init();
    }

    public void OnDisable()
    {
        timer.OnTimerEnded -= EndFarmCycle;
    }

    private static void Init()
    {
        //gameStart?.Invoke(); // ideally this should start the whole farm sequence+UI but i not sure how exactly it will happen so for now it runs on start
        saveData = SaveSystem.LoadGame();
        Debug.Log($"current farm level = {saveData.currentLevel}");
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
        StartFarmCycle?.Invoke();
        StartGridPlanting?.Invoke(levelData);
        SetUpcomingEnemies?.Invoke(levelData);
        cycleStarted = true;
        //AudioService.AudioManager.PlayOneShot(debugMusic);
        //AudioService.AudioManager.SetVolume(0.1f);
    }
    public void EndFarmCycle()
    {
        stopGame = true;
        cycleStarted = false;
        StopFarmCycle?.Invoke();
    }

}