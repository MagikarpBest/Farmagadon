using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private WeaponInventory weaponInventory;

    [Header("UI Elements (match by slot index)")]
    [SerializeField] private TextMeshProUGUI[] ammoTexts; // Array of text fields (1 per weapon slot)
    [SerializeField] private Image[] ammoIcons;           // Array of icons (1 per weapon slot)

    private void OnEnable()
    {
        ammoInventory.OnInventoryChanged += UpdateUI;
    }
    private void OnDisable()
    {
        ammoInventory.OnInventoryChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Updates all ammo slot UI elements to match the player's current weapons and ammo counts.
    /// </summary>
    private void UpdateUI()
    {
        // Loop through all weapon slots
        for (int i = 0; i < ammoTexts.Length; i++)
        {
            // Get weapon currently equipped in this slot
            WeaponSlot weaponSlot = weaponInventory.GetWeaponSlot(i);

            if (weaponSlot != null && weaponSlot.weaponData != null)
            {
                // Check which ammo this weapon uses
                AmmoData ammoType = weaponSlot.weaponData.ammoType;

                if (ammoType != null)
                {
                    // Get current ammo count from inventory
                    int count = ammoInventory.GetAmmoCount(ammoType);

                    // Update text with ammo name and count
                    ammoTexts[i].text = $"{ammoType.ammoName}: {count}";

                    // Update ammo icon
                    if (ammoIcons != null && i < ammoIcons.Length)
                    {
                        ammoIcons[i].enabled = true;
                        ammoIcons[i].sprite = ammoType.icon;
                    }
                }
                else
                {
                    // Weapon has no ammo assigned
                    ammoTexts[i].text = "No Ammo";
                    if (ammoIcons != null && i < ammoIcons.Length)
                    {
                        ammoIcons[i].enabled = false;
                    }
                }
            }
            else
            {
                // No weapon equipped on that slot
                ammoTexts[i].text = "Empty Slot";
                if (ammoIcons != null && i < ammoIcons.Length)
                {
                    ammoIcons[i].enabled = false;
                }
            }
        }
    }
}
