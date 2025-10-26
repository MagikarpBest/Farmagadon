using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanelHandler : MonoBehaviour
{
    [SerializeField] private FarmController farmController;
    [SerializeField] private AmmoDatabase ammoDatabase;
    [SerializeField] private BulletPanelDatabase bulletPanelDatabase;

    private Image[] bulletPanels = new Image[4];
    private float prevY;
    private float yOffset = -200.0f;
    
    private void OnEnable()
    {
        farmController.StartFarmCycle += UpdateAmmoList;
        
    }

    private void OnDisable()
    {
        farmController.StartFarmCycle -= UpdateAmmoList;
    }

    private void Start()
    {
        UpdateAmmoList();
    }

    private void UpdateAmmoList()
    {
        if (bulletPanels.Length > 0)
        {
            DeleteBulletPanels();
        }

        WeaponInventory wepInv = farmController.WeaponInventory;
        AmmoInventory ammoInv = farmController.AmmoInventory;
        
        for (int i = 0; i < wepInv.getWeaponsSize()-1; i++)
        {
            WeaponSlot wepSlot = wepInv.GetWeaponSlot(i);
            if (wepSlot == null) { continue; }
            WeaponData wepData = wepInv.GetWeaponSlot(i).weaponData;
            if (wepData == null) { break; }
            BulletPanelData bulletPanelData = bulletPanelDatabase.GetBulletPanelDataByWeaponID(wepData.weaponID);
            if (bulletPanelData == null) { break; }
            
            Image bulletPanelPrefab = Instantiate(bulletPanelData.bulletPanel, this.transform);
            bulletPanels[i] = bulletPanelPrefab;
            if (i > 0) 
            {
                bulletPanelPrefab.rectTransform.anchoredPosition = new Vector2(bulletPanelPrefab.rectTransform.anchoredPosition.x, bulletPanelPrefab.rectTransform.anchoredPosition.y + (yOffset * i));
            }
            bulletPanelPrefab.GetComponent<BulletPanelUpdater>().AmmoData = wepData.ammoType;
            bulletPanelPrefab.GetComponent<BulletPanelUpdater>().AmmoInventory = ammoInv;
            bulletPanelPrefab.GetComponent<BulletPanelUpdater>().SetImage(wepData.ammoType.cropIcon);
            farmController.OnCropFarmed += bulletPanelPrefab.GetComponent<BulletPanelUpdater>().UpdateSelf;
            bulletPanelPrefab.GetComponent<BulletPanelUpdater>().UpdateSelf();
        }
        
    }


   
    private void DeleteBulletPanels()
    {
        for (int i = bulletPanels.Length-1; i>=0; --i)
        {
            Destroy(bulletPanels[i]);
        }
        bulletPanels = new Image[4];
    }
    
     
}
