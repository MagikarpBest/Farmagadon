using UnityEngine;
using System.Collections.Generic;
using Farm;
public class GameController : MonoBehaviour
{
    [SerializeField] private GridController gridController;
    [SerializeField] private AmmoInventory ammoInventory;

    public delegate void StartGame();
    public StartGame gameStart;
    public delegate void CropFarmed();
    public CropFarmed OnCropFarmed;
    public delegate void SetRecommended(ENEMY_WEAKNESS[] placeholderParam);
    public SetRecommended OnGetRecommended;

    

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
        }
        else if (cropNames == CROP_NAMES.Carrot)
        {
            ammoInventory.AddAmmo(carrotAmmo, dropAmount);
        }
        else if (cropNames == CROP_NAMES.Potato) 
        {
            ammoInventory.AddAmmo(potatoAmmo, dropAmount);
        }
        OnCropFarmed?.Invoke();
    }
    public void Start()
    {
        gameStart?.Invoke();
        OnCropFarmed?.Invoke();
        ENEMY_WEAKNESS[] testList = { ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice, ENEMY_WEAKNESS.Rice};
        OnGetRecommended?.Invoke(testList);
    }

   
}
