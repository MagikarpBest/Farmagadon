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

    [Header("UI Elements")]
    [SerializeField] private List<Image> inventoryImages;               // Weapon icons on inventory
    [SerializeField] private List<Image> equippedImages;                // Loadout equipped image
    [SerializeField] private List<TextMeshProUGUI> ammoTexts;           // Ammo count text
    [SerializeField] private List<TextMeshProUGUI> weaponDescription;   
    [SerializeField] private Sprite emptySlotSprite;

    private List<WeaponSlot> allOwned;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        
        // Get all owned weapon and put into a list
        allOwned = weaponInventory.GetAllOwnedWeapons();
        UpdateInventoryVisual();
        UpdateEquippedVisual();
        UpdateSelectedDescription();
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
        List<WeaponSlot> equipped = weaponInventory.GetEquippedWeapon();

        for (int i = 0; i < equippedImages.Count; i++)
        {
            Image icon = equippedImages[i];

            if (i < equipped.Count && equipped[i] != null && equipped[i].weaponData != null)
            {
                icon.sprite = equipped[i].weaponData.weaponSprite;
            }
            else
            {
                icon.sprite = emptySlotSprite;
            }
        }
    }

    private void UpdateSelectedDescription()
    {

    }
}
