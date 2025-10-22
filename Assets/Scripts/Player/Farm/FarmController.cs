using UnityEngine;
using System.Collections.Generic;
using Farm;
public class FarmController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    public AmmoInventory AmmoInventory { get { return ammoInventory; } }

    private bool stopGame = false;
    public bool StopGame { get { return stopGame; } set { stopGame = value; } }

    public delegate void StartGame();
    public StartGame gameStart;
    public delegate void EndGame();
    public EndGame gameEnd; // invoked from Timer.cs
    public delegate void CropFarmed();
    public CropFarmed OnCropFarmed; // invoked from plants.cs -> GridController.cs
    public delegate void SetRecommended(ENEMY_WEAKNESS[] placeholderParam); // ignore for now
    public SetRecommended OnGetRecommended;



    public void cropFarmed(AmmoData cropName, int dropAmount)
    {
        ammoInventory.AddAmmo(cropName, dropAmount); // add to ammo inv is here
        OnCropFarmed?.Invoke(); // this one connects to BulletPanelHandler, just to update the UI


    }

    public void Start()
    {
        gameStart?.Invoke(); // ideally this should start the whole farm sequence+UI but i not sure how exactly it will happen so for now it runs on start
        //ENEMY_WEAKNESS[] testList = { ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice}; these 2 ignore for now. havent converted to SO yet
        //OnGetRecommended?.Invoke(testList);
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