using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    // Save file location
    private static string savePath = Application.persistentDataPath + "/save.json";

    // Save
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved at: " + savePath);
    }

    // Load
    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from : " + savePath);
            return data;
        }
        Debug.Log("No save data found. Creating new save.");
        return new SaveData() { currentLevel = 1, currentPhase = GamePhase.Farm };
    }

    // Check whether you have a save file or not
    public static bool HasSaveData()
    {
        return File.Exists(savePath);
    }

    // Clear save file (idk maybe you want to create new save file)
    public static void ClearSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted: " + savePath);
        }
    }   
}
