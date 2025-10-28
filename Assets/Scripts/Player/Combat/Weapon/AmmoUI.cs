using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;


public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AmmoInventory ammoInventory; 
    [SerializeField] private WeaponInventory weaponInventory;

    [Header("UI Elements (slot order: Center, Right, Bottom, Left)")]
    [SerializeField] private TextMeshProUGUI centerWeaponText;
    [SerializeField] private TextMeshProUGUI rightWeaponText;
    [SerializeField] private TextMeshProUGUI bottomWeaponText;
    [SerializeField] private TextMeshProUGUI leftWeaponText;    // 4th slot 

    private Action<WeaponSlot> weaponChangedHandler;

    private void OnEnable()
    {
        if(ammoInventory!=null)
        { 
            ammoInventory.OnInventoryChanged += UpdateUI;
        }
        
        if (weaponInventory!=null)  
        {
            // Store the lambda reference to remove later
            weaponChangedHandler = (slot) => UpdateUI();
            weaponInventory.OnWeaponChanged += weaponChangedHandler;
        }
    }
    
    private void OnDisable()
    {
        if (ammoInventory != null)
        {
            ammoInventory.OnInventoryChanged -= UpdateUI;
        }

        if (weaponInventory != null)
        {
            weaponInventory.OnWeaponChanged -= weaponChangedHandler;
        }
    }

    private void Start()
    {
        // Delay 0.1 second before initialize incase bugs
        Invoke(nameof(UpdateUI), 0.1f);
    }

    /// <summary>
    /// Updates all ammo slot UI elements to match the player's current weapons and ammo counts.
    /// </summary>
    private void UpdateUI()
    {
        if (weaponInventory == null || ammoInventory == null) 
        {
            Debug.LogWarning("[AmmoUI] Missing inventory reference.");
            return;
        }

        // Get all slots from weapon inventory
        int totalSlots = weaponInventory.UnlockedSlotCount;
        List<WeaponSlot> allSlots = new();

        for (int i = 0; i < totalSlots; i++) 
        {
            allSlots.Add(weaponInventory.GetWeaponSlot(i)); // may include nulls (locked or empty)
        }

        // if there’s nothing at all, just clear UI
        if (allSlots.Count == 0)
        {
            SetEmptyAll();
            return;
        }

        // Find current weapon
        WeaponSlot currentWeaponSlot = weaponInventory.GetWeaponSlot(weaponInventory.GetCurrentWeaponIndex());
        if (currentWeaponSlot == null)
        {
            SetEmptyAll();
            return;
        }

        int currentIndex = allSlots.IndexOf(currentWeaponSlot);
        if (currentIndex < 0) 
        {
            currentIndex = 0; // safety fallback
        }

        // Wrap-around neighbors (same as WeaponUI order)
        WeaponSlot centerSlot = allSlots[currentIndex];
        WeaponSlot rightSlot = allSlots[(currentIndex + 1) % totalSlots];
        WeaponSlot bottomSlot = allSlots[(currentIndex + 2) % totalSlots];
        WeaponSlot leftSlot = allSlots[(currentIndex + 3) % totalSlots];

        // Update UI texts
        SetText(centerWeaponText, centerSlot);
        SetText(rightWeaponText, rightSlot);
        SetText(bottomWeaponText, bottomSlot);
        SetText(leftWeaponText, leftSlot);
    }

    /// <summary>
    /// Sets text for a specific slot (shows ammo count or N/A)
    /// </summary>
    private void SetText(TextMeshProUGUI textElement, WeaponSlot slot)
    {
        if (textElement == null)
        {
            return;
        }

        if (slot != null && slot.weaponData != null && slot.weaponData.ammoType != null) 
        {
            int count = ammoInventory.GetAmmoCount(slot.weaponData.ammoType);
            textElement.text = $"{count}";
        }
        else
        {
            textElement.text = "N/A";
        }
    }

    /// <summary>
    /// Set every text in ammoUi to N/A
    /// </summary>
    private void SetEmptyAll()
    {
        if (centerWeaponText != null) centerWeaponText.text = "N/A";
        if (rightWeaponText != null) rightWeaponText.text = "N/A";
        if (bottomWeaponText != null) bottomWeaponText.text = "N/A";
        if (leftWeaponText != null) leftWeaponText.text = "N/A";
    }
}
