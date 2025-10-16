using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// All player data that needed to be saved
/// </summary>
[System.Serializable]
public class SaveData
{
    // --------------------------
    // Progress Data
    // --------------------------
    public int currentLevel = 1;
    public GamePhase currentPhase = GamePhase.Farm;

    // --------------------------
    // Weapon Inventory
    // --------------------------
    public int unlockedSlots = 4;
    public List<string> ownedWeaponIDs = new();     // Store owned weapon ID/names
    public List<string> equippedWeaponIDs = new();  // What’s currently in active slots

    // --------------------------
    // Ammo Inventory
    // --------------------------
    public List<string> ownedAmmoIDs = new();
    public List<int> ownedAmmoCounts = new();

    // -----------------
    // Constructor
    // -----------------
    public SaveData() 
    { 
    
    }
}
