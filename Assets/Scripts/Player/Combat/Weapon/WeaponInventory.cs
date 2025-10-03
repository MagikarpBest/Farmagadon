using UnityEngine;
using System;


[System.Serializable]
public class WeaponSlot
{
    public WeaponData weaponData;

    public WeaponSlot(WeaponData data)
    {
        weaponData = data;
    }
}

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private int maxSlot = 4;
    [SerializeField] private int unlockedSlot = 2;
    [SerializeField] private WeaponData[] startingWeapon;

    private WeaponSlot[] weapons;
    private AmmoInventory ammoInventory;
    private int currentIndex = 0;

    public event Action<WeaponSlot> OnWeaponChanged;

    private void Awake()
    {
        weapons = new WeaponSlot[maxSlot];
        ammoInventory = GetComponent<AmmoInventory>();
        if (ammoInventory == null)
        {
            Debug.LogError("No AmmoInventory found on player!");
        }
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
                weapons[i] = new WeaponSlot(newWeapon);
                currentIndex = i;
                OnWeaponChanged?.Invoke(weapons[i]);
                return;
            }
        }
        Debug.Log("All slots full. Need to swap!");
    }

    public void SwapWeapon(int slotIndex, WeaponData newWeapon)
    {
        if (slotIndex < unlockedSlot)
        {
            weapons[slotIndex] = new WeaponSlot(newWeapon);
            currentIndex = slotIndex;
            OnWeaponChanged?.Invoke(weapons[slotIndex]);
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

    public WeaponSlot GetCurrentWeapon()
    {
        return weapons[currentIndex];
    }
}
