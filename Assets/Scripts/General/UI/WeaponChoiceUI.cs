using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChoiceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] Button[] choiceButtons;
    [SerializeField] TextMeshProUGUI[] labels;        // Display for weapon names

    private Action<string> onWeaponChosen;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Show(List<string> weaponIDs, Action<string> onChosen)
    {
        if (panel == null)
        {
            Debug.LogError("WeaponChoiceUIManager: No panel assigned!");
            return;
        }

        if (weaponIDs == null || weaponIDs.Count == 0)
        {
            Debug.LogWarning("WeaponChoiceUIManager: No weapon options provided!");
            return;
        }

        onWeaponChosen = onChosen;
        panel.SetActive(true);

        // Setup button options
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < weaponIDs.Count)
            {
                string weaponID = weaponIDs[i];
                choiceButtons[i].gameObject.SetActive(true);

                // Update button label if available
                if (labels != null && i < labels.Length && labels[i] != null)
                {
                    labels[i].text = weaponID;
                }

                // Reset previous listeners and add new one
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ChooseWeapon(weaponID));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Called when the player picks a weapon button.
    /// </summary>
    private void ChooseWeapon(string weaponID)
    {
        panel.SetActive(false);
        onWeaponChosen.Invoke(weaponID);
        onWeaponChosen = null;
    }
}
