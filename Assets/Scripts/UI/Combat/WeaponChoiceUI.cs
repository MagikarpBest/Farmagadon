using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChoiceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private GameObject[] rewardCards;
    [SerializeField] private Image[] rewardIcons;
    [SerializeField] private TextMeshProUGUI[] weaponNames;        // Display for weapon names
    [SerializeField] private WeaponDatabase weaponDatabase;

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
        for (int i = 0; i < rewardCards.Length; i++)
        {
            if (i < weaponIDs.Count)
            {
                string weaponID = weaponIDs[i];
                var data = weaponDatabase.GetWeaponByID(weaponID);
                rewardCards[i].SetActive(true);

                // Update button label if available
                if (weaponNames != null && i < weaponNames.Length && weaponNames[i] != null)
                {
                    weaponNames[i].text = data.weaponName;
                }

                // Set weapon icon from database
                if (rewardIcons != null && i < rewardIcons.Length && data != null && data.weaponSprite)
                {
                    rewardIcons[i].sprite = data.weaponSprite;
                }

                // Reset previous listeners and add new one
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ChooseWeapon(weaponID));
            }
            else
            {
                rewardCards[i].gameObject.SetActive(false);
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
