using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanelHandler : MonoBehaviour
{
    // --- 1. Singleton Instance ---
    // This provides safe, static-like access without the static list bug
    public static BulletPanelHandler Instance { get; private set; }

    [SerializeField] private FarmController farmController;
    [SerializeField] private AmmoDatabase ammoDatabase;
    [SerializeField] private Image bulletPanelPrefab;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    // --- 2. The list is NO LONGER STATIC ---
    // This list will now be destroyed and re-created with this script
    private List<PanelData> bulletPanels = new();

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

    // --- 3. Awake() to set the Singleton ---
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this one
            Destroy(gameObject);
        }
        else
        {
            // This is the one and only instance
            Instance = this;
        }
    }

    private void OnEnable()
    {
        farmController.StartFarmCycle += UpdateAmmoList;
    }

    private void OnDisable()
    {
        farmController.StartFarmCycle -= UpdateAmmoList;

        // Clean up panels and event subscriptions
        ClearAllPanels();
    }

    // --- 4. The getter is NO LONGER STATIC ---
    public Image GetBulletPanel(AmmoData data)
    {
        foreach (var panelData in bulletPanels)
        {
            // Check if the panel reference itself is still valid
            if (panelData.bulletPanel != null && panelData.ammoData == data)
            {
                return panelData.bulletPanel;
            }
        }

        // --- 5. Return NULL if not found ---
        // This stops the "fly to the wrong panel" bug
        return null;
    }

    private void UpdateAmmoList()
    {
        // --- 6. Always clear old panels before creating new ones ---
        // This stops duplicates and "ghost" objects when re-loading the scene
        ClearAllPanels();

        WeaponInventory wepInv = farmController.WeaponInventory;
        AmmoInventory ammoInv = farmController.AmmoInventory;

        foreach (KeyValuePair<AmmoData, int> ammoDict in ammoInv.AmmoDict)
        {
            AmmoData ammoData = ammoDict.Key;
            string ammoID = ammoData.ammoID;

            // Only create panels for these specific ammo types
            if (ammoID == "ammo_carrot" || ammoID == "ammo_corn" || ammoID == "ammo_potato")
            {
                Image bulletPanelObject = Instantiate(bulletPanelPrefab, verticalLayoutGroup.transform);

                // Add the new, valid panel to the now-empty list
                bulletPanels.Add(new PanelData(bulletPanelObject, ammoData));

                BulletPanelUpdater updater = bulletPanelObject.GetComponent<BulletPanelUpdater>();
                if (updater != null)
                {
                    updater.AmmoData = ammoData;
                    updater.AmmoInventory = ammoInv;
                    updater.SetImage(ammoData.cropIcon);

                    // Subscribe and update
                    farmController.OnCropFarmed += updater.UpdateSelf;
                    updater.UpdateSelf();
                }
            }
        }
    }

    // --- 7. New helper method to safely destroy UI and unsubscribe ---
    private void ClearAllPanels()
    {
        foreach (var panelData in bulletPanels)
        {
            if (panelData.bulletPanel != null)
            {
                // Must unsubscribe from events before destroying
                BulletPanelUpdater updater = panelData.bulletPanel.GetComponent<BulletPanelUpdater>();
                if (updater != null)
                {
                    farmController.OnCropFarmed -= updater.UpdateSelf;
                }

                Destroy(panelData.bulletPanel.gameObject);
            }
        }

        bulletPanels.Clear();
    }
}