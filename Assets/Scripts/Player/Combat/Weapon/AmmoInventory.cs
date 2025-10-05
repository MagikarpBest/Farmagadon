using UnityEngine;
using System.Collections.Generic;
using System;

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

public class AmmoInventory : MonoBehaviour
{
    public event Action OnInventoryChanged;

    // Create a collection that store all ammo data
    [SerializeField] private AmmoData[] startAmmoTypes;
    [SerializeField] private int debugStartingAmmo = 50;
    

    private Dictionary<AmmoData, int>
    ammoDict = new Dictionary<AmmoData, int>();

    private void Start()
    {
        foreach (var ammo in startAmmoTypes)
        {
            if (ammo != null)
            {
                AddAmmo(ammo, debugStartingAmmo);
            }
        }
    }
    public void AddAmmo(AmmoData ammo, int amount)
    {
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

    public bool ConsumeAmmo(AmmoData ammo, int amount)
    {
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
}
