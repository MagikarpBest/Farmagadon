using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    [SerializeField] private AmmoInventory inventory;

    [SerializeField] private Image[] ammoImages;
    [SerializeField] private TextMeshProUGUI[] ammoName;
    [SerializeField] private TextMeshProUGUI[] ammoCountText;
    private AmmoData currentSelectedAmmo;

    public void OpenCraft(AmmoData currentSlot)
    {
        currentSelectedAmmo = currentSlot;
        DisplayRequirement();
        Debug.Log("Crafting popup opened for: " + currentSelectedAmmo.ammoName);
    }

    public void Craft()
    {
        if (currentSelectedAmmo == null)
        {
            Debug.LogWarning("[CraftManager] Get no ammodata");
            return;
        }

        if (!currentSelectedAmmo.canBeCrafted)
        {
            Debug.Log(currentSelectedAmmo.ammoName + " cannot be crafted.");
            return;
        }

        // Check if player have enough ingredient
        foreach (var required in currentSelectedAmmo.craftingRequirements)
        {
            int owned = inventory.GetAmmoCount(required.ammo);

            if (owned < required.amountNeeded)
            {
                Debug.Log($"Not enough{required.ammo.ammoName} (Owned:{owned}/ Required:{required.amountNeeded}");  
                return;
            }
        }

        // Deduct required ammo
        foreach (var required in currentSelectedAmmo.craftingRequirements)
        {
            inventory.AddAmmo(required.ammo, -required.amountNeeded);
        }

        // Add ammo for crafted ammo
        inventory.AddAmmo(currentSelectedAmmo, currentSelectedAmmo.amountProduced);
        Debug.Log($"Crafted {currentSelectedAmmo.amountProduced} {currentSelectedAmmo.ammoName}");

        // Refresh UI
        DisplayRequirement();
    }
    
    private void DisplayRequirement()
    {
        ammoImages[0].sprite = currentSelectedAmmo.craftingRequirements[0].ammo.icon;
        ammoImages[1].sprite = currentSelectedAmmo.craftingRequirements[1].ammo.icon;
        ammoImages[2].sprite = currentSelectedAmmo.icon;
        return;
        ammoName[0].text = $"{currentSelectedAmmo.craftingRequirements[0].ammo.ammoName}";
        ammoName[1].text = $"{currentSelectedAmmo.craftingRequirements[1].ammo.ammoName}";
        ammoName[2].text = $"{currentSelectedAmmo.ammoName}";

        ammoCountText[0].text = $"{currentSelectedAmmo.craftingRequirements[0].amountNeeded}/{inventory.GetAmmoCount(currentSelectedAmmo.craftingRequirements[0].ammo)}";
        ammoCountText[1].text = $"{currentSelectedAmmo.craftingRequirements[1].amountNeeded}/{inventory.GetAmmoCount(currentSelectedAmmo.craftingRequirements[1].ammo)}";
        ammoCountText[2].text = $"{currentSelectedAmmo.amountProduced}/{inventory.GetAmmoCount(currentSelectedAmmo)}";
    }
}