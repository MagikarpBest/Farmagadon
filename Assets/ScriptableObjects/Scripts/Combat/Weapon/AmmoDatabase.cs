using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AmmoDatabase", menuName = "Game/Ammo Database")]
public class AmmoDatabase : ScriptableObject
{
    [Header("All Ammo Types")]
    public AmmoData[] allAmmo;

    /// <summary>
    /// Finds and returns an ammo by its unique ID.
    /// </summary>
    public AmmoData GetAmmoByID(string id)
    {
        foreach (var ammo in allAmmo)
        {
            if (ammo != null && ammo.ammoID == id)
            {
                return ammo;
            }
        }
        Debug.LogWarning($"Ammo with ID {id} not found in AmmoDatabase!");
        return null;
    }
}
