using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles level completion reward (choose one or auto give)
/// </summary>
public class LevelRewardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private UIManager uiManager;

    [Header("Rewards Data")]
    [SerializeField] private LevelRewardData rewardData; // Optional, can be empty if you randomize

    private void Awake()
    {
        ValidateReference();
    }

    /// <summary>
    /// Check is any of the refenrece missing
    /// </summary>
    private void ValidateReference()
    {
        Debug.Assert(weaponInventory != null, $"{name}: Missing WeaponInventory reference!");
        Debug.Assert(weaponDatabase != null, $"{name}: Missing WeaponDatabase reference!");
        Debug.Assert(uiManager != null, $"{name}: Missing UIManager reference!");
        Debug.Assert(rewardData != null, $"{name}: Missing LevelRewardData reference!");
        Debug.Assert(weaponInventory != null, $"{name}: Missing WeaponInventory reference!");
        Debug.Assert(weaponInventory != null, $"{name}: Missing WeaponInventory reference!");
    }


    public void GetRewardForLevel(int level)
    {
        // Search the rewardData list and find the reward entry that matches the given level number
        if (rewardData == null)
        {
            Debug.LogError("RewardData is null — cannot give rewards!");
            return;
        }

        // Find reward for this level
        var reward = rewardData.rewards.Find(rewards => rewards.levelNumber == level);
        if (reward == null)
        {
            Debug.LogWarning($"No reward set for this level{level}");
        }

        // Filter out already owned weapons
        var availableRewards = GetUnownedWeapons(reward.rewardWeaponIDs);
        if (availableRewards.Count == 0)
        {
            Debug.Log($"Player already owned all rewards");
            return;
        }

        if (reward.chooseOne)
        {
            // Pick up to 3 random unique choices
            int choiceCount = Mathf.Min(3, availableRewards.Count);
            List<string> randomChoices = new List<string>();

            // Copy list to avoid modifying the original
            List<string> pool = new List<string>(availableRewards);

            for (int i = 0; i < choiceCount; i++)
            {
                int randomIndex = Random.Range(0, pool.Count);
                randomChoices.Add(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
            }

            // Show a weapon choice popup
        }
        else
        {
            // Give all reward directly
            foreach (var id in availableRewards)
            {
                WeaponData weapon = weaponDatabase.GetWeaponByID(id);
                if (weapon != null)
                {
                    weaponInventory.AddWeapon(weapon);
                    Debug.Log($"Granted weapon: {weapon.weaponName}");
                }
            }
        }
    }

    /// <summary>
    /// Filters out owned or equipped weapons
    /// </summary>
    private List<string> GetUnownedWeapons(List<string> rewardWeaponIDs)
    {
        var unowned = new List<string>();

        foreach (var id in rewardWeaponIDs)
        {
            bool alreadyOwned = weaponInventory.IsWeaponOwned(id);
            if (!alreadyOwned)
            {
                unowned.Add(id);
            }
        }
        return unowned;
    }

    /// <summary>
    /// Called when player selects a weapon from the choice UI.
    /// </summary>
    private void OnWeaponChosen(string chosenID)
    {
        WeaponData weapon = weaponDatabase.GetWeaponByID(chosenID);
        if (weapon != null)
        {
            weaponInventory.AddWeapon(weapon);
            Debug.Log($"Player chose reward: {weapon.weaponName}");
        }
        else
        {
            Debug.LogWarning($"Invalid weapon ID chosen: {chosenID}");
        }
    }
}