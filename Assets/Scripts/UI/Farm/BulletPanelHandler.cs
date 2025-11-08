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

    private static PanelData[] bulletPanels = new PanelData[4];
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
        return null;
    }

    private void UpdateAmmoList()
    {
        if (bulletPanels.Length > 0)
        {
            //DeleteBulletPanels();
        }

        WeaponInventory wepInv = farmController.WeaponInventory;
        AmmoInventory ammoInv = farmController.AmmoInventory;
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

        }
    }

    private void DeleteBulletPanels()
    {
        if (bulletPanels.Length <= 0) { return; }
        for (int i = bulletPanels.Length - 1; i >= 0; --i)
        {
            if (bulletPanels[i].bulletPanel == null) { continue; }
            Destroy(bulletPanels[i].bulletPanel);
        }
        bulletPanels = new PanelData[4];
    }



}