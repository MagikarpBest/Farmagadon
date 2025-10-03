using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private AmmoData[] ammoTypes;
    [SerializeField] private TextMeshProUGUI[] ammoTexts;

    private void OnEnable()
    {
        ammoInventory.OnInventoryChanged += UpdateUI;
    }
    private void OnDisable()
    {
        ammoInventory.OnInventoryChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < ammoTypes.Length && i < ammoTexts.Length; i++)
        {
            int count = ammoInventory.GetAmmoCount(ammoTypes[i]);
            ammoTexts[i].text = $"{ammoTypes[i].ammoName}: {count}";
        }
    }
}
