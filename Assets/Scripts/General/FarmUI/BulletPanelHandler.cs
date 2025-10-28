using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanelHandler : MonoBehaviour
{
    [SerializeField] private FarmController farmController;
    [SerializeField] private AmmoDatabase ammoDatabase;
    [SerializeField] private Image bulletPanelPrefab;

    private Image[] bulletPanels = new Image[4];
    private float yOffset = -200.0f;
    
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
        int removeOffset = 0;
        for (int i = 0; i < wepInv.getWeaponsSize(); ++i)
        {
            
            WeaponSlot wepSlot = wepInv.GetWeaponSlot(i);
            if (wepSlot == null) { continue; }
            if (wepSlot.weaponData.weaponID == "weapon_rice") { removeOffset = -1; continue; }
            WeaponData wepData = wepInv.GetWeaponSlot(i).weaponData;
            if (wepData == null) { break; }

            
            Image bulletPanelObject = Instantiate(bulletPanelPrefab, this.transform);
            bulletPanels[i] = bulletPanelObject;
            if (i > 0) 
            {
                bulletPanelObject.rectTransform.anchoredPosition = new Vector2(bulletPanelObject.rectTransform.anchoredPosition.x, bulletPanelObject.rectTransform.anchoredPosition.y + (yOffset * (i+removeOffset)));
            }
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
