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

public enum WeaponSwitchDirection
{
    None,
    Next,
    Previous
}

/// <summary>
/// Manages the player's weapon inventory handles adding, swapping, and switching between weapons.
/// </summary>
public class WeaponInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private AmmoInventory ammoInventory;       // Reference to player's ammo inventory

    [Header("Slot Settings")]
    [SerializeField] private int maxSlot = 4;                   // Maximum number of weapon slots the player can have
    [SerializeField] private int unlockedSlots = 1;             // How many weapon slots currently player have
    [SerializeField] private WeaponData[] startingWeapon;       // Weapons player start with
    [SerializeField] private float swapDelay = 0.25f;           // Follow swap animation

    private SaveData saveData;
    private WeaponSlot[] weapons;                               // Assign weapon to slots, first assign = first slot

    private List<WeaponSlot> weaponStorage = new();             // Reserve weapons not equipped
    private int currentIndex = 0;                               // The index of current active weapon slot
    private bool canSwitchWeapon = true;

    public event Action OnWeaponLoadedFromSave;
    /// <summary>
    /// Event triggered whenever the currently equipped weapon changes.
    /// </summary>  
    public event Action<WeaponSlot, WeaponSwitchDirection> OnWeaponChanged;

    private void Awake()
    {
        // Initialize weapon slot array
        weapons = new WeaponSlot[maxSlot];

        if (ammoInventory == null)
        {
            Debug.LogError("No AmmoInventory found on player!");
        }
    }

    private void Start()
    {
        saveData = SaveSystem.LoadGame();
    }

    #region Slot Management
    /// <summary>
    /// Unlocks one additional weapon slot (up to the max slot limit).
    /// </summary>
    // Ngl this is useless for now
    public void UnlockSlot()
    {
        if (unlockedSlots < maxSlot) 
        {
            unlockedSlots++;
        }
    }

    public void EquipWeapon(WeaponSlot newWeapon)
    {
        if (unlockedSlots <= 0)
        {
            Debug.LogWarning("[WeaponInventory] No unlocked slot");
            return;
        }

        // Prevent equipping if it is already equipped
        foreach (var slot in weapons)
        {
            if (slot != null && slot.weaponData != null && slot.weaponData.weaponID == newWeapon.weaponData.weaponID)
            {
                Debug.Log("Weapon already equipped.");
                return;
            }
        }

        // Step 1: Find the FIRST empty slot among unlocked slots
        int targetSlot = -1;

        for (int i = 0; i < unlockedSlots; i++)
        {
            if (weapons[i] == null)
            {
                targetSlot = i;
                break;
            }
        }

        // Step 2: If no empty slot, block the equip
        if (targetSlot == -1)
        {
            Debug.Log("Cannot equip, all equipped slots are full!");
            return;
        }

        // Step 3: Equip new weapon
        weapons[targetSlot] = new WeaponSlot(newWeapon.weaponData);
        currentIndex = targetSlot;

        Debug.Log($"Equipped {newWeapon.weaponData.weaponName} into slot {targetSlot}");
    }

    public void UnEquipWeaponByID(string weaponID)
    {
        for (int i = 0; i < unlockedSlots; i++)
        {
            if (weapons[i] != null && weapons[i].weaponData != null && weapons[i].weaponData.weaponID == weaponID)
            {
                string weaponName = weapons[i].weaponData.weaponName;
                weapons[i] = null;
                Debug.Log($"Unequipped {weaponName} from slot {i}");
                return;
            }
        }
        Debug.LogWarning($"Weapon {weaponID} not found in equipped slots.");
    }

    /// <summary>
    /// Adds a new weapon to the first available unlocked slot.
    /// If all unlocked slots are full, send to reserve inventory;
    /// </summary>
    public void AddWeapon(WeaponData newWeapon)
    {
        // Prevent duplicate weapons
        if (IsWeaponOwned(newWeapon.weaponID))
        {
            return;
        }

        // Add normally
        for (int i = 0; i < unlockedSlots; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = new WeaponSlot(newWeapon);
                currentIndex = i;
                if (currentIndex == i)
                {
                    OnWeaponChanged?.Invoke(weapons[currentIndex], WeaponSwitchDirection.None);
                }
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
        if (slotIndex < unlockedSlots)
        {
            weapons[slotIndex] = new WeaponSlot(newWeapon);
            currentIndex = slotIndex;
            OnWeaponChanged?.Invoke(weapons[slotIndex], WeaponSwitchDirection.None);
        }
    }

    /// <summary>
    /// Switches to the next available weapon slot (wraps around).
    /// </summary>
    public void NextWeapon()
    {
        if (!canSwitchWeapon)
        {
            return;
        }

        canSwitchWeapon = false;
        PerformNextWeapon();
        Invoke(nameof(ResetSwitch), swapDelay); // prevent new swaps until animation ends
    }

    /// <summary>
    /// Switches to the previous available weapon slot (wraps around).
    /// </summary>
    public void PreviousWeapon()
    {
        if (!canSwitchWeapon)
        {
            return;
        }

        canSwitchWeapon = false;
        PerformPreviousWeapon();
        Invoke(nameof(ResetSwitch), swapDelay); // prevent new swaps until animation ends
    }

    private void PerformNextWeapon()
    {
        int startIndex = currentIndex;

        // Skip empty slots
        do
        {
            currentIndex = (currentIndex + 1) % unlockedSlots;
        }
        while (weapons[currentIndex] == null && currentIndex != startIndex);

        OnWeaponChanged?.Invoke(weapons[currentIndex], WeaponSwitchDirection.Next);
    }

    private void PerformPreviousWeapon()
    {
        int startIndex = currentIndex;

        // Skip empty slots
        do
        {
            currentIndex = (currentIndex - 1 + unlockedSlots) % unlockedSlots;
        } while (weapons[currentIndex] == null && currentIndex != startIndex);

        OnWeaponChanged?.Invoke(weapons[currentIndex], WeaponSwitchDirection.Previous);
    }

    private void ResetSwitch()
    {
        canSwitchWeapon = true;
    }

    /// <summary>
    /// Check is the weapon requested for whatever usage already owned or not
    /// </summary>
    /// 
    public bool IsWeaponOwned(string weaponID)
    {
        foreach (var slot in weapons)
        {
            if (slot != null && slot.weaponData != null && slot.weaponData.weaponID == weaponID)
            {
                Debug.Log($"Weapon {slot.weaponData.weaponName} already equipped!");
                return true;
            }
        }

        foreach (var weapon in weaponStorage)
        {
            if (weapon != null && weapon.weaponData != null && weapon.weaponData.weaponID == weaponID)
            {
                Debug.Log($"Weapon {weapon.weaponData.weaponName} already reserved!");
                return true;
            }
        }
        return false;
    }
    #endregion Slot Management

    #region Save System
    public void SaveToSaveData(SaveData data)
    {
        saveData = data;
        SaveWeaponsToSaveData();
    }

    /// <summary>
    /// Starting weapon logic for new players and initialize from save
    /// </summary>
    public void InitializeFromSave(SaveData data)
    {
        saveData = data;
        LoadWeaponFromSave();

        // Give default weapon if empty (for first time player)
        if (GetOnlyEquippedWeapon().Count == 0)
        {
            foreach (var weapon in startingWeapon)
            {
                if (weapon != null)
                {
                    AddWeapon(weapon);
                }
            }
        }
    }

    /// <summary>
    /// Save equipped + reserve weapons to SaveData
    /// </summary>
    private void SaveWeaponsToSaveData()
    {
        saveData.ownedWeaponIDs.Clear();
        saveData.equippedWeaponIDs.Clear();

        HashSet<string> uniqueIDs = new HashSet<string>();

        // Save equipped weapons (no duplicates)
        foreach (var slot in GetOnlyEquippedWeapon())
        {
            if (slot.weaponData != null && uniqueIDs.Add(slot.weaponData.weaponID))
            {
                saveData.equippedWeaponIDs.Add(slot.weaponData.weaponID);
                saveData.ownedWeaponIDs.Add(slot.weaponData.weaponID);
            }
        }

        // Save reserve weapons
        foreach (var weapon in weaponStorage)
        {
            if (weapon.weaponData != null && uniqueIDs.Add(weapon.weaponData.weaponID))
            {
                saveData.ownedWeaponIDs.Add(weapon.weaponData.weaponID);
            }
        }

        // Save unlocked slots
        saveData.unlockedSlots = unlockedSlots;

        SaveSystem.SaveGame(saveData);
        Debug.Log("Weapons saved to SaveData.");
    }

    /// <summary>
    /// Load equipped + reserve weapons from SaveData
    /// </summary>
    private void LoadWeaponFromSave()
    {
        if (saveData == null)
        {
            Debug.LogWarning("No save data found. Staring new inventory");
            return;
        }

        unlockedSlots = saveData.unlockedSlots;

        // Load equipped
        for (int i = 0; i < saveData.equippedWeaponIDs.Count && i < unlockedSlots; i++) 
        {
            WeaponData weapon = weaponDatabase.GetWeaponByID(saveData.equippedWeaponIDs[i]);
            if (weapon != null)
            {
                weapons[i]= new WeaponSlot(weapon);
            }
        }

        // Load reserve
        foreach (var id in saveData.ownedWeaponIDs)
        {
            WeaponData weapon = weaponDatabase.GetWeaponByID(id);
            if (weapon != null)
            {
                weaponStorage.Add(new WeaponSlot(weapon));
            }
        }
        Debug.Log("Weapons loaded from SaveData.");

        if (weapons != null && weapons[0] != null && weapons[0].weaponData != null)
        {
            currentIndex = 0;
            OnWeaponChanged?.Invoke(weapons[currentIndex], WeaponSwitchDirection.None);
            OnWeaponLoadedFromSave?.Invoke();
        }
        else
        {
            Debug.LogWarning("[WeaponInventory] No valid weapon found after loading save data.");
        }
    }
    #endregion Save System

    #region Getters
    /// <summary>
    /// Returns the weapon slot at the specified index.
    /// Returns null if the index is out of range.
    /// </summary>
    public WeaponSlot[] GetWeaponSlots()
    {
        return weapons;
    }
    /// <summary>
    /// Returns the weapon slot at the specified index.
    /// Returns null if the index is out of range.
    /// </summary>
   
    public WeaponSlot GetWeaponSlotOfSpecificIndex(int index)
    {
        if (index >= 0 && index < weapons.Length)
        {
            return weapons[index];
        }
        return null;
    }

    public bool IsWeaponEquipped(WeaponSlot weaponSlot)
    {
        // Prevent equipping if it is already equipped
        foreach (var slot in weapons)
        {
            if (slot != null && slot.weaponData != null && slot.weaponData.weaponID == weaponSlot.weaponData.weaponID)
            {
                Debug.Log("Weapon already equipped.");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns a list of all non-empty (equipped) weapon slots.
    /// </summary>
    public List<WeaponSlot> GetOnlyEquippedWeapon()
    {
        if (weapons == null || weapons.Length == 0)
            return new List<WeaponSlot>();

        var equipped = new List<WeaponSlot>();
        for (int i = 0; i < Mathf.Min(unlockedSlots, weapons.Length); i++)
        {
            if (weapons[i] != null && weapons[i].weaponData != null)
            {
                equipped.Add(weapons[i]);
            }
        }

        return equipped;
    }

    public List<WeaponSlot> GetAllOwnedWeapons()
    {
        return weaponStorage;
    }

    /// <summary>
    /// Returns the number of unlocked slots.
    /// </summary>
    public int UnlockedSlotCount => unlockedSlots;

    /// <summary>
    /// Returns the index of the currently active weapon.
    /// </summary>
    public int GetCurrentWeaponIndex() => currentIndex;

    public bool CanSwitchWeapon => canSwitchWeapon; // Read-only
    #endregion Getters
}
