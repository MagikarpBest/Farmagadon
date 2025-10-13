using UnityEngine;
using System.Collections.Generic;
using Farm;
public class GameController : MonoBehaviour
{
    [SerializeField] FarmUIController UIController;
    [SerializeField] GridController gridController;
    [SerializeField] AmmoInventory ammoInventory;
    
    public delegate void StartGame();
    public StartGame gameStart;

    private int totalCorn = 0;
    private int totalCarrot = 0;
    private int totalPotato = 0;

    private void Awake()
    {
        
    }
    public void cropFarmed(CROP_NAMES cropNames, int dropAmount)
    {
        AmmoData carrotAmmo = ammoInventory.StartAmmoTypes[0];
        AmmoData cornAmmo = ammoInventory.StartAmmoTypes[1];
        AmmoData potatoAmmo = ammoInventory.StartAmmoTypes[2];
        if (cropNames == CROP_NAMES.Corn)
        {
            ammoInventory.AddAmmo(cornAmmo, dropAmount);
            totalCorn = ammoInventory.GetAmmoCount(cornAmmo);
        }
        else if (cropNames == CROP_NAMES.Carrot)
        {
            ammoInventory.AddAmmo(carrotAmmo, dropAmount);
            totalCarrot = ammoInventory.GetAmmoCount(carrotAmmo);
        }
        else if (cropNames == CROP_NAMES.Potato) 
        {
            ammoInventory.AddAmmo(potatoAmmo, dropAmount);
            totalPotato = ammoInventory.GetAmmoCount(potatoAmmo);
        }
        UIController.updateBulletCount(totalCorn, totalCarrot, totalPotato);
    }
    public void Start()
    {
        cropFarmed(CROP_NAMES.Corn,0);
        cropFarmed(CROP_NAMES.Carrot, 0);
        cropFarmed(CROP_NAMES.Potato, 0);
        gameStart?.Invoke();
    }

   
}
