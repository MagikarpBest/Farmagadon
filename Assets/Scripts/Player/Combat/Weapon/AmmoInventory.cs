using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Represent ammo type and count
/// </summary>
[System.Serializable]
public class AmmoSlot
{
    public AmmoData ammoData;
    public int count;

    public AmmoSlot(AmmoData data, int startingCount)
    {
        ammoData = data;
        count = startingCount;
    }
}

/// <summary>
/// Manages all ammo owned by Player
/// Handles adding, consume and saving ammo
/// </summary>
public class AmmoInventory : MonoBehaviour
{
    public event Action OnInventoryChanged;

    [Header("Database Reference")]
    [SerializeField] private AmmoDatabase ammoDatabase;

    [Header("Starting Ammo (Debug)")]
    // Create a collection that store all ammo data
    [SerializeField] private AmmoData[] startAmmoTypes;
    [SerializeField] private int debugStartingAmmo = 50;

    // Runtime ammo data storage
    private Dictionary<AmmoData, int> ammoDict = new Dictionary<AmmoData, int>();
    private SaveData saveData;

    // ----------------------
    // Initialization
    // ----------------------
    private void Start()
    {
        
    }

    // ----------------------
    // Inventory Management
    // ----------------------
    /// <summary>
    /// Add ammo into the inventory (could be use for after harvest etc etc)
    /// </summary
    public void AddAmmo(AmmoData ammo, int amount)
    {
        if (ammo == null)
        {
            return;
        }
        Debug.Log($"[AmmoInventory] Adding {amount}x {ammo.ammoName} ({ammo.GetInstanceID()})");

        // Check if the dictionary added the ammo in or not, if not create new list?(idk whats the terms) and add ammo
        if (ammoDict.ContainsKey(ammo))
        {
            ammoDict[ammo] += amount;
        }
        else
        {
            ammoDict.Add(ammo, amount);
        }
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Consume ammo, used for weapon shooting
    /// </summary>
    public bool ConsumeAmmo(AmmoData ammo, int amount)
    {
        if (ammo == null)
        {
            return false;
        }

        if (ammoDict.TryGetValue(ammo, out int current) && current >= amount)
        {
            ammoDict[ammo] -= amount;
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public int GetAmmoCount(AmmoData ammo)
    {
        ammoDict.TryGetValue (ammo, out int count);
        return count;
    }

    // ----------------------
    // Save System
    // ----------------------
    public void SaveToSaveData(SaveData data)
    {
        saveData = data;

        saveData.ownedAmmoIDs.Clear();
        saveData.ownedAmmoCounts.Clear();

        foreach (var kvp in ammoDict)
        {
            if (kvp.Key != null)
            {
                saveData.ownedAmmoIDs.Add(kvp.Key.ammoID);
                saveData.ownedAmmoCounts.Add(kvp.Value);
            }
        }
    }
    /// <summary>
    /// Starting ammo initialize from save
    /// </summary>
    public void InitializeFromSave(SaveData data)
    {
        saveData = data; // First, store the save data
        // Load saved ammo first
        LoadAmmoFromSave();

        // Give debug starting ammo if empty (for testing)
        if (ammoDict.Count == 0)
        {
            foreach (var ammo in startAmmoTypes)
            {
                if (ammo != null)
                {
                    AddAmmo(ammo, debugStartingAmmo);
                }
            }
        }
    }

    /// <summary>
    /// Load equipped + reserve ammo from SaveData
    /// </summary>
    private void LoadAmmoFromSave()
    {
        ammoDict.Clear();

        if (saveData == null)
        {
            Debug.LogWarning("No save data found. Starting new ammo inventory.");
            return;
        }

        for (int i = 0; i < saveData.ownedAmmoIDs.Count; i++)
        {
            string id = saveData.ownedAmmoIDs[i];
            int count = saveData.ownedAmmoCounts[i];

            AmmoData ammo = ammoDatabase.GetAmmoByID(id);
            if (ammo != null)
            {
                ammoDict[ammo] = count;
            }
        }

        Debug.Log("Ammo loaded from SaveData.");
    }
}
