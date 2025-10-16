using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a weapon slot that holds a specific weapon data.
/// </summary>
[System.Serializable]
public class WeaponSlot
{
    public WeaponData weaponData;
    public WeaponSlot(WeaponData data)
    {
        weaponData = data;
    }
}

/// <summary>
/// Manages the player's weapon inventory handles adding, swapping, and switching between weapons.
/// </summary>
public class WeaponInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponDatabase weaponDatabase;
    private AmmoInventory ammoInventory;                    // Reference to player's ammo inventory
    private SaveData saveData;

    [Header("Slot Settings")]
    [SerializeField] private int maxSlot = 4;               // Maximum number of weapon slots the player can have
    [SerializeField] private int unlockedSlot = 1;          // How many weapon slots currently player have
    [SerializeField] private WeaponData[] startingWeapon;   // Weapons player start with

    private WeaponSlot[] weapons;                           // Assign weapon to slots, first assign = first slot
    private List<WeaponSlot> weaponStorage = new();         // Reserve weapons not equipped
    private int currentIndex = 0;                           // The index of current active weapon slot

    /// <summary>
    /// Event triggered whenever the currently equipped weapon changes.
    /// </summary>
    public event Action<WeaponSlot> OnWeaponChanged;

    private void Awake()
    {
        // Initialize weapon slot array
        weapons = new WeaponSlot[maxSlot];

        // Cache the player's ammo inventory component
        ammoInventory = GetComponent<AmmoInventory>();
        if (ammoInventory == null)
        {
            Debug.LogError("No AmmoInventory found on player!");
        }
    }

    private void Start()
    {
        saveData = SaveSystem.LoadGame();
        
        // Add all starting weapon to available slots
        foreach (var weapon in startingWeapon)
        {
            if (weapon != null)
            {
                AddWeapon(weapon);
            }
        }

        // Set first weapon (slots 0) as the default active weapon
        currentIndex = 0;
        OnWeaponChanged?.Invoke(weapons[currentIndex]);
    }

    // ----------------------
    // Slot Management
    // ----------------------

    /// <summary>
    /// Unlocks one additional weapon slot (up to the max slot limit).
    /// </summary>
    public void UnlockSlot()
    {
        if (unlockedSlot < maxSlot) 
        {
            unlockedSlot++;
        }
    }

    /// <summary>
    /// Adds a new weapon to the first available unlocked slot.
    /// If all unlocked slots are full, send to reserve inventory;
    /// </summary>
    public void AddWeapon(WeaponData newWeapon)
    {
        for (int i = 0; i < unlockedSlot; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = new WeaponSlot(newWeapon);
                currentIndex = i;
                OnWeaponChanged?.Invoke(weapons[i]);

                return;
            }
        }
        weaponStorage.Add(new WeaponSlot(newWeapon));
        Debug.Log($"All equipped slot full! {newWeapon.weaponName} sent to reserve inventory.");
    }

    /// <summary>
    /// Replaces an existing weapon in a specific slot.
    /// </summary>
    public void SwapWeapon(int slotIndex, WeaponData newWeapon)
    {
        if (slotIndex < unlockedSlot)
        {
            weapons[slotIndex] = new WeaponSlot(newWeapon);
            currentIndex = slotIndex;
            OnWeaponChanged?.Invoke(weapons[slotIndex]);
        }
    }

    /// <summary>
    /// Switches to the next available weapon slot (wraps around).
    /// </summary>
    public void NextWeapon()
    {
        int startIndex = currentIndex;

        // Skip empty slots
        do
        {
            currentIndex = (currentIndex + 1) % unlockedSlot;
        }
        while (weapons[currentIndex] == null && currentIndex != startIndex);

        OnWeaponChanged?.Invoke(weapons[currentIndex]);
    }

    /// <summary>
    /// Switches to the previous available weapon slot (wraps around).
    /// </summary>
    public void PreviousWeapon()
    {
        int startIndex = currentIndex;

        // Skip empty slots
        do
        {
            currentIndex = (currentIndex - 1 + unlockedSlot) % unlockedSlot;
        }
        while (weapons[currentIndex] == null && currentIndex != startIndex);

        OnWeaponChanged?.Invoke(weapons[currentIndex]);
    }

    /// <summary>
    /// Returns the weapon slot at the specified index.
    /// Returns null if the index is out of range.
    /// </summary>
    public WeaponSlot GetWeaponSlot(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            return weapons[index];
        }
        return null;
    }

    /// <summary>
    /// Returns a list of all non-empty (equipped) weapon slots.
    /// </summary>
    public List<WeaponSlot> GetEquippedWeapon()
    {
        var equipped= new List<WeaponSlot>();
        for (int i = 0; i < unlockedSlot; i++)
        {
            if (weapons[i] != null && weapons[i].weaponData != null)
            {
                equipped.Add(weapons[i]);
            }
        }

        return equipped;
    }


    /// <summary>
    /// Returns the number of unlocked slots.
    /// </summary>
    public int UnlockedSlotCount => unlockedSlot;

    /// <summary>
    /// Returns the index of the currently active weapon.
    /// </summary>
    public int GetCurrentWeaponIndex() => currentIndex;

    // ----------------------
    // Save System
    // ----------------------

    /// <summary>
    /// Save equipped + reserve weapons to SaveData
    /// </summary>
}
