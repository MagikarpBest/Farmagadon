using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelRewardData", menuName = "Game/Level Reward Data")] 
public class LevelRewardData : ScriptableObject
{
    [System.Serializable]
    public class LevelReward
    {
        public int levelNumber;
        ///public List<string> rewardWe
    }
}
