using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftManager : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private List<CraftingRecipe> recipes;
    [SerializeField] private Button craftButton;

    private CraftingRecipe currentRecipe;

    private void Start()
    {
        SelectRecipe(recipes[0]);
        if (craftButton != null)
            craftButton.onClick.AddListener(TryCraft);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        currentRecipe = recipe;
    }
    public int GetAmmoCountBySlot(WeaponSlot slot)
    {
        if (slot == null || slot.weaponData == null || slot.weaponData.ammoType == null)
            return 0;

        return ammoInventory.GetAmmoCount(slot.weaponData.ammoType);
    }
    private void TryCraft()
    {
        Debug.Log("try harder");
        if (currentRecipe == null)
        {
            Debug.Log("recipe null");
            return;
        }

        for (int i = 0; i < currentRecipe.requiredAmmo.Count; i++)
        {
            AmmoData ammo = currentRecipe.requiredAmmo[i];
            int requiredCount = currentRecipe.requiredCounts[i];

            if (ammoInventory.GetAmmoCount(ammo) < requiredCount)
            {
                Debug.Log($"Missing {requiredCount}x {ammo.ammoName}");
                return;
            }
        }

        // Consume required ammo
        for (int i = 0; i < currentRecipe.requiredAmmo.Count; i++)
        {
            AmmoData ammo = currentRecipe.requiredAmmo[i];
            int count = currentRecipe.requiredCounts[i];
            ammoInventory.ConsumeAmmo(ammo, count);
        }

        // Add crafted result
        ammoInventory.AddAmmo(currentRecipe.resultAmmo, currentRecipe.resultCount);
        Debug.Log($"Crafted {currentRecipe.resultCount}x {currentRecipe.resultAmmo.ammoName}");
    }
}