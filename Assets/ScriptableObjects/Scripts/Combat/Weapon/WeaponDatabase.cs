using UnityEngine;

/// <summary>
/// Holds references to all WeaponData assets in the game.
/// Lets you look up weapons by their unique ID.
/// </summary>

[CreateAssetMenu(fileName ="WeaponDatabase",menuName ="Game/Weapon Database")]
public class WeaponDatabase : ScriptableObject
{
    [Header("All Weapon Type")]
    public WeaponData[] allWeapons;

    /// <summary>
    /// Finds and returns a weapon by its unique ID.
    /// </summary>
    public WeaponData GetWeaponByID(string id)
    {
        foreach (var weapon in allWeapons)
        {
            if (weapon != null && weapon.weaponID == id) 
            {
                return weapon;
            }
        }
        Debug.LogWarning($"Weapon with ID '{id}' not found in WeaponDatabase!");
        return null;
    }
}
