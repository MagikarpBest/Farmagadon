using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Reward list for each levels
/// </summary>
[CreateAssetMenu(fileName = "LevelRewardData", menuName = "Game/Level Reward Data")] 
public class LevelRewardData : ScriptableObject
{
    [System.Serializable]
    public class LevelReward
    {
        public int levelNumber;
        public List<string> rewardWeaponIDs;
        public bool chooseOne;
    }
    public List<LevelReward> rewards;
}
