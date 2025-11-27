using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private LoadOutManager loadoutManager;

    [Header("UI Elements")]
    [SerializeField] private List<Image> inventoryImages;               // Weapon icons on inventory
    [SerializeField] private List<Image> equippedImages;                // Loadout equipped image
    [SerializeField] private List<TextMeshProUGUI> ammoTexts;           // Ammo count text
    [SerializeField] private TextMeshProUGUI weaponDescription;   
    [SerializeField] private Sprite emptySlotSprite;

    private List<WeaponSlot> allOwned;


    private void OnEnable()
    {
        if (loadoutManager != null)
        {
            loadoutManager.OnInventorySlotChanged += UpdateSelectedDescription;
            loadoutManager.OnTriggerUIUpdate += UpdateAll;
        }
    }
    private void OnDisable()
    {
        if (loadoutManager != null)
        {
            loadoutManager.OnInventorySlotChanged -= UpdateSelectedDescription;
            loadoutManager.OnTriggerUIUpdate -= UpdateAll;
        }
    }

    private IEnumerator Start()
    {
        //yield return new WaitForSeconds(0.5f);
        
        // Get all owned weapon and put into a list
        allOwned = weaponInventory.GetAllOwnedWeapons();
        UpdateInventoryVisual();
        UpdateEquippedVisual();
        UpdateSelectedDescription(0);
        yield return null;
    }

    private void UpdateAll()
    {
        UpdateInventoryVisual();
        UpdateEquippedVisual();
    }

    private void UpdateInventoryVisual()
    {
        for (int i = 0; i < inventoryImages.Count; i++)
        {
            Image icon = inventoryImages[i];
            TextMeshProUGUI ammoText = ammoTexts[i];

            // Display all weapon in list, If null show empty sprite and text
            if (i < allOwned.Count && allOwned[i] != null && allOwned[i].weaponData != null) 
            {
                WeaponSlot slot = allOwned[i];
                icon.sprite = slot.weaponData.weaponSprite;

                if (slot.weaponData.ammoType != null)
                {
                    ammoText.text = $"{ammoInventory.GetAmmoCount(slot.weaponData.ammoType)}";
                }
                else
                {
                    ammoText.text = "N/A";
                }
            }
            else
            {
                icon.sprite = emptySlotSprite;
                ammoText.text = "N/A";
            }
        }
    }

    private void UpdateEquippedVisual()
    {
        WeaponSlot[] slots = weaponInventory.GetWeaponSlots();

        for (int i = 0; i < equippedImages.Count; i++)
        {
            Image icon = equippedImages[i];

            if (i < slots.Length && slots[i] != null && slots[i].weaponData != null)
            {
                icon.sprite = slots[i].weaponData.weaponSprite;
            }
            else
            {
                icon.sprite = emptySlotSprite;
            }
        }
    }

    private void UpdateSelectedDescription(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= allOwned.Count)
        {
            weaponDescription.text = "NULL";
            return;
        }

        WeaponSlot selectedSlot = allOwned[selectedIndex];
        if (selectedSlot == null || selectedSlot.weaponData == null)
        {
            weaponDescription.text = "";
            return;
        }

        weaponDescription.text = selectedSlot.weaponData.weaponDescription;
    }
}
