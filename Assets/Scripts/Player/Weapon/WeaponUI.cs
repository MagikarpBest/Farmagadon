using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] WeaponInventory weaponInventory;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] TextMeshProUGUI weaponAmmoText;

    private void OnEnable()
    {
        weaponInventory.OnWeaponChanged += UpdateUI;
        weaponInventory.OnAmmoChanged += UpdateUI;
    }
    private void OnDisable()
    {
        weaponInventory.OnWeaponChanged -= UpdateUI;
        weaponInventory.OnAmmoChanged += UpdateUI;
    }

    private void UpdateUI(WeaponSlot slot)
    {
        if (slot != null && slot.weaponData != null)
        {
            weaponNameText.text = slot.weaponData.weaponName;
            weaponAmmoText.text = $"Ammo:{slot.currentAmmo}";
        }
        else
        {
            weaponNameText.text = "No Weapon";
            weaponAmmoText.text = "No Ammo";
        }
    }
}
