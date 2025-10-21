using TMPro;
using UnityEngine;

public class BulletPanelHandler : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private AmmoDatabase ammoDatabase;
    [SerializeField] private TextMeshProUGUI carrotText;
    [SerializeField] private TextMeshProUGUI cornText;
    [SerializeField] private TextMeshProUGUI potatoText;


    
    private void OnEnable()
    {
        gameController.OnCropFarmed += updateAmmoList;
        
    }

    private void OnDisable()
    {
        gameController.OnCropFarmed -= updateAmmoList;
    }
    
    private void updateAmmoList(AmmoData cropName)
    {
        AmmoInventory ammoInv = gameController.AmmoInventory;
        carrotText.text = "X " + ammoInv.GetAmmoCount(ammoDatabase.GetAmmoByID("ammo_carrot"));
        cornText.text = "X " + ammoInv.GetAmmoCount(ammoDatabase.GetAmmoByID("ammo_corn"));
        potatoText.text = "X " + ammoInv.GetAmmoCount(ammoDatabase.GetAmmoByID("ammo_potato"));

    }
   
    private void Start()
    {
        AmmoInventory ammoInv = gameController.AmmoInventory;
        carrotText.text = "X " + ammoInv.GetAmmoCount(ammoDatabase.GetAmmoByID("ammo_carrot"));
        cornText.text = "X " + ammoInv.GetAmmoCount(ammoDatabase.GetAmmoByID("ammo_corn"));
        potatoText.text = "X " + ammoInv.GetAmmoCount(ammoDatabase.GetAmmoByID("ammo_potato"));
    }
     
}
