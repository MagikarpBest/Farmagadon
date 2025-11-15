using System.Collections.Generic;
using System.Linq;
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

    private static List<PanelData> bulletPanels = new();
    private struct PanelData
    {
        public Image bulletPanel;
        public AmmoData ammoData;

        public PanelData(Image panel, AmmoData data)
        {
            bulletPanel = panel;
            ammoData = data;
        }
    }
    private void OnEnable()
    {

        farmController.StartFarmCycle += UpdateAmmoList;
    }

    private void OnDisable()
    {
        farmController.StartFarmCycle -= UpdateAmmoList;
    }

    public static Image GetBulletPanel(AmmoData data)
    {
        foreach (var panelData in bulletPanels)
        {
            if (panelData.ammoData == data)
            {
                return panelData.bulletPanel;
            }
        }
        return bulletPanels[0].bulletPanel;
    }

    private void UpdateAmmoList()
    {

        WeaponInventory wepInv = farmController.WeaponInventory;
        AmmoInventory ammoInv = farmController.AmmoInventory;

        foreach (KeyValuePair<AmmoData, int> ammoDict in ammoInv.AmmoDict)
        {
            
            AmmoData ammoData = ammoDict.Key;
            string ammoID = ammoData.ammoID;
            print($"AmmoData: {ammoID}, Ammo Amount: {ammoDict.Value}");
            if (ammoID == "ammo_carrot" || ammoID == "ammo_corn" || ammoID == "ammo_potato") 
            {
                Image bulletPanelObject = Instantiate(bulletPanelPrefab, verticalLayoutGroup.transform);
                bulletPanels.Add(new PanelData(bulletPanelObject, ammoData));
                bulletPanelObject.GetComponent<BulletPanelUpdater>().AmmoData = ammoData;
                bulletPanelObject.GetComponent<BulletPanelUpdater>().AmmoInventory = ammoInv;
                bulletPanelObject.GetComponent<BulletPanelUpdater>().SetImage(ammoData.cropIcon);
                farmController.OnCropFarmed += bulletPanelObject.GetComponent<BulletPanelUpdater>().UpdateSelf;
                bulletPanelObject.GetComponent<BulletPanelUpdater>().UpdateSelf();
            }
        }

        /*
        for (int i = 0; i < wepInv.getWeaponsSize(); ++i)
        {
            
            WeaponSlot wepSlot = wepInv.GetWeaponSlot(i);

            if (wepSlot == null) { continue; }
            if (wepSlot.weaponData.weaponID == "weapon_rice") { continue; }
            WeaponData wepData = wepInv.GetWeaponSlot(i).weaponData;
            if (wepData == null) { break; }


            Image bulletPanelObject = Instantiate(bulletPanelPrefab, verticalLayoutGroup.transform);
            bulletPanels[i] = new PanelData(bulletPanelObject, wepData.ammoType);
            bulletPanelObject.GetComponent<BulletPanelUpdater>().AmmoData = wepData.ammoType;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().AmmoInventory = ammoInv;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().SetImage(wepData.ammoType.cropIcon);
            farmController.OnCropFarmed += bulletPanelObject.GetComponent<BulletPanelUpdater>().UpdateSelf;
            bulletPanelObject.GetComponent<BulletPanelUpdater>().UpdateSelf();

        }*/
    }




}