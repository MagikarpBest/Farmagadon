using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private WeaponInventory weaponInventory;

    [Header("UI Elements (match by slot index)")]
    [SerializeField] private TextMeshProUGUI[] ammoTexts;
    [SerializeField] private Image[] ammoIcons;

    private void OnEnable()
    {
        ammoInventory.OnInventoryChanged += UpdateUI;
    }
    private void OnDisable()
    {
        ammoInventory.OnInventoryChanged -= UpdateUI;
    }

    private void Start()
    {
        UpdateUI();
    }


    private void UpdateUI()
    {
        for (int i = 0; i < ammoTexts.Length; i++)
        {
            WeaponSlot weaponSlot = weaponInventory.GetWeaponSlot(i);

            if (weaponSlot != null && weaponSlot.weaponData != null)
            {
                AmmoData ammoType = weaponSlot.weaponData.ammoType;

                if (ammoType != null)
                {
                    int count = ammoInventory.GetAmmoCount(ammoType);
                    ammoTexts[i].text = $"{ammoType.ammoName}: {count}";

                    if (ammoIcons != null && i < ammoIcons.Length)
                    {
                        ammoIcons[i].enabled = true;
                        ammoIcons[i].sprite = ammoType.icon;
                    }
                }
                else
                {
                    ammoTexts[i].text = "No Ammo";
                    if (ammoIcons != null && i < ammoIcons.Length)
                    {
                        ammoIcons[i].enabled = false;
                    }
                }
            }
            else
            {
                ammoTexts[i].text = "Empty Slot";
                if (ammoIcons != null && i < ammoIcons.Length)
                {
                    ammoIcons[i].enabled = false;
                }
            }
        }
    }
}
