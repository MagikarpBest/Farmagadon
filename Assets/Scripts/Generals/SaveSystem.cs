using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private const string SaveKey = "GameSaveData";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static SaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded: " + json);
            return data;
        }
        Debug.Log("No save data found. Creating new save.");
        return new SaveData() { currentLevel = 1 };
    }

    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        Debug.Log("Save data cleared");
    }
}
