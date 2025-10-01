using UnityEngine;
using System;


[System.Serializable]
public class WeaponSlot
{
    public WeaponData weaponData;
    public int currentAmmo;

    public WeaponSlot(WeaponData data, int startingAmmo)
    {
        weaponData = data;
        currentAmmo = startingAmmo;
    }
}

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private int maxSlot = 4;
    [SerializeField] private int unlockedSlot = 2;
    [SerializeField] private WeaponData[] startingWeapon;
    [SerializeField] private int defaultStartingAmmo = 30;

    private WeaponSlot[] weapons;
    private int currentIndex = 0;

    public event Action<WeaponSlot> OnWeaponChanged;
    public event Action<WeaponSlot> OnAmmoChanged;

    private void Awake()
    {
        weapons = new WeaponSlot[maxSlot];
    }

    private void Start()
    {
        foreach (var weapon in startingWeapon)
        {
            if (weapon != null)
            {
                AddWeapon(weapon, defaultStartingAmmo);
            }
        }
        
        for (int i = 0; i < unlockedSlot; i++)
        {
            if (weapons[i] != null)
            {
                currentIndex = i;
                OnWeaponChanged?.Invoke(weapons[i]);
                break;
            }
        }
    }

    public void UnlockSlot()
    {
        if (unlockedSlot < maxSlot) 
        {
            unlockedSlot++;
        }
    }

    public void AddWeapon(WeaponData newWeapon, int startingAmmo)
    {
        for (int i = 0; i < unlockedSlot; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = new WeaponSlot(newWeapon, startingAmmo);
                currentIndex = i;
                OnWeaponChanged?.Invoke(weapons[i]);
                return;
            }
        }
        Debug.Log("All slots full. Need to swap!");
    }

    public void SwapWeapon(int slotIndex, WeaponData newWeapon, int startingAmmo)
    {
        if (slotIndex < unlockedSlot)
        {
            weapons[slotIndex] = new WeaponSlot(newWeapon, startingAmmo);
            currentIndex = slotIndex;
            OnWeaponChanged?.Invoke(weapons[slotIndex]);
        }
    }

    public bool ConsumeAmmo(int amount)
    {
        var slot = GetCurrentWeapon();
        if (slot == null || slot.weaponData == null)
        {
            return false;
        }

        if (slot.currentAmmo >= amount)
        {
            slot.currentAmmo -= amount;
            OnAmmoChanged.Invoke(slot);
            return true;
        }
        return false;
    }

    public void NextWeapon()
    {
        int startIndex = currentIndex;

        do
        {
            currentIndex = (currentIndex + 1) % unlockedSlot;
        }
        while (weapons[currentIndex] == null && currentIndex != startIndex);

        OnWeaponChanged?.Invoke(weapons[currentIndex]);
    }

    public void PreviousWeapon()
    {
        int startIndex = currentIndex;

        do
        {
            currentIndex = (currentIndex - 1 + unlockedSlot) % unlockedSlot;
        }
        while (weapons[currentIndex] == null && currentIndex != startIndex);

        OnWeaponChanged?.Invoke(weapons[currentIndex]);
    }

    public WeaponSlot GetCurrentWeapon()
    {
        return weapons[currentIndex];
    }
}
