using UnityEngine;

public class FarmDataBridge : MonoBehaviour
{
    [SerializeField] private FarmController farmController;

    private SaveData saveData;

    // Called by GameManager to inject loaded data
    public void Initialize(SaveData data)
    {
        saveData = data;

        if (farmController != null)
        {
            farmController.InitializeFromSave(saveData);
            Debug.Log("[FarmDataBridge] Initialized FarmController from SaveData.");
        }
        else
        {
            Debug.LogError("[FarmDataBridge] farmController not assigned!");
        }
    }

    // Called when farm session ends (before switching scenes)
    public void SaveProgress()
    {
        if (farmController == null || saveData == null)
        {
            Debug.LogError("[FarmDataBridge] Cannot save — missing references");
            return;
        }

        farmController.ApplyProgressToSave(saveData);
        SaveSystem.SaveGame(saveData);
        Debug.Log("[FarmDataBridge] Farm progress applied and saved.");
    }
}