using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelRewardManager rewardManager;
    [SerializeField] private SceneController sceneController;
    private SaveData saveData;

    /// <summary>
    /// Get runtime save
    /// </summary>
    public void InitializeFromSave(SaveData runtimeSave)
    {
        saveData = runtimeSave;
    }

    /// <summary>
    /// Call rewardManager for reward, increase current level and save progress.
    /// </summary>
    public void CompleteLevel()
    {
        
        // Give reward for current level
        rewardManager.GetRewardForLevel(saveData.currentLevel);

        // If current phase is combat change to farm, if farm then change to combat
        saveData.currentPhase = saveData.currentPhase == GamePhase.Combat
                                                      ? GamePhase.Farm 
                                                      : GamePhase.Combat;

        if (saveData.currentPhase == GamePhase.Farm)
        {
            saveData.currentLevel++;
        }

        // Save runtime progress
        SaveSystem.SaveGame(saveData);
        Debug.Log($"current level {saveData.currentLevel}");
    }
}
