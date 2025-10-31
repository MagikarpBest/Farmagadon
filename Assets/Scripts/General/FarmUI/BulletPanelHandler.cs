using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanelHandler : MonoBehaviour
{
    [SerializeField] private FarmController farmController;
    [SerializeField] private AmmoDatabase ammoDatabase;
    [SerializeField] private Image bulletPanelPrefab;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    private Image[] bulletPanels = new Image[4];
    
    private void OnEnable()
    {
        farmController.StartFarmCycle += UpdateAmmoList;
    }

    private void OnDisable()
    {
        farmController.StartFarmCycle -= UpdateAmmoList;
    }

    private void UpdateAmmoList()
    {
        if (bulletPanels.Length > 0)
        {
            DeleteBulletPanels();
        }

        WeaponInventory wepInv = farmController.WeaponInventory;
        AmmoInventory ammoInv = farmController.AmmoInventory;
        for (int i = 0; i < wepInv.getWeaponsSize(); ++i)
        {
            
            WeaponSlot wepSlot = wepInv.GetWeaponSlot(i);
<<<<<<< HEAD
            if (wepSlot == null) { continue; }
            if (wepSlot.weaponData.weaponID == "weapon_rice") { removeOffset = -1; continue; }
=======

            if (wepSlot == null) { continue; }
            if (wepSlot.weaponData.weaponID == "weapon_rice") { continue; }
>>>>>>> origin/28/10-chris
            WeaponData wepData = wepInv.GetWeaponSlot(i).weaponData;
            if (wepData == null) { break; }

            
            Image bulletPanelObject = Instantiate(bulletPanelPrefab, verticalLayoutGroup.transform);
            bulletPanels[i] = bulletPanelObject;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().AmmoData = wepData.ammoType;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().AmmoInventory = ammoInv;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().SetImage(wepData.ammoType.cropIcon);
            farmController.OnCropFarmed += bulletPanelObject.GetComponent<BulletPanelUpdater>().UpdateSelf;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().UpdateSelf();
        }
    }

    private void DeleteBulletPanels()
    {
        if (bulletPanels.Length <= 0) { return; }
        for (int i = bulletPanels.Length-1; i>=0; --i)
        {
            Destroy(bulletPanels[i]);
        }
        bulletPanels = new Image[4];
    }
    
     
}
