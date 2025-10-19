using TMPro;
using UnityEngine;

public class BulletPanelHandler : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] AmmoInventory ammoInv;
    [SerializeField] TextMeshProUGUI carrotText;
    [SerializeField] TextMeshProUGUI cornText;
    [SerializeField] TextMeshProUGUI potatoText;



    private void OnEnable()
    {
        gameController.OnCropFarmed += updateAmmoList;
        
    }

    private void OnDisable()
    {
        gameController.OnCropFarmed -= updateAmmoList;
    }

    private void updateAmmoList()
    {
        AmmoData carrotAmmo = ammoInv.StartAmmoTypes[0];
        AmmoData cornAmmo = ammoInv.StartAmmoTypes[1];
        AmmoData potatoAmmo = ammoInv.StartAmmoTypes[2];
        carrotText.text = "X " + ammoInv.GetAmmoCount(carrotAmmo).ToString();
        cornText.text = "X " + ammoInv.GetAmmoCount(cornAmmo).ToString();
        potatoText.text = "X " + ammoInv.GetAmmoCount(potatoAmmo).ToString();
        

    }

    private void Start()
    {
        updateAmmoList();
    }
}
