using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int currentLevel = 1;
    public GamePhase currentPhase = GamePhase.Farm;
    public int unlockedSlots = 4;
    public List<string> ownedWeaponIDs = new();     // Store owned weapon ID/names
    public List<string> equippedWeaponIDs = new();  // What’s currently in active slots
}
