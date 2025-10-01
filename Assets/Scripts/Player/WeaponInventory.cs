using UnityEngine;
using System;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private int maxSlot = 4;
    [SerializeField] private int unlockedSlot = 2;
    [SerializeField] private WeaponData[] startingWeapon;

    private WeaponData[] weapons;
    private int currentIndex = 0;

    public event Action<WeaponData> OnWeaponChanged;

    private void Awake()
    {
        weapons = new WeaponData[maxSlot];
    }

    private void Start()
    {
        foreach (var weapon in startingWeapon)
        {
            if (weapon != null)
            {
                AddWeapon(weapon);
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

    public void AddWeapon(WeaponData newWeapon)
    {
        for (int i = 0; i < unlockedSlot; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = newWeapon;
                currentIndex = i;
                OnWeaponChanged?.Invoke(newWeapon);
                return;
            }
        }
        Debug.Log("All slots full. Need to swap!");
    }

    public void SwapWeapon(int slotIndex, WeaponData newWeapon)
    {
        if (slotIndex < unlockedSlot)
        {
            weapons[slotIndex] = newWeapon;
            currentIndex = slotIndex;
            OnWeaponChanged?.Invoke(newWeapon);
        }
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

    public WeaponData GetCurrentWeapon()
    {
        return weapons[currentIndex];
    }
}
