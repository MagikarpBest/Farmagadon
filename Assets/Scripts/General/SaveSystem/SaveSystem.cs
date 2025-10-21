using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    // The file path where the save data will be stored
    // Application.persistentDataPath is a Unity-provided folder that is writable on all platforms
    // Example (Windows): C:/Users/UserName/AppData/LocalLow/YourCompany/YourGame/save.json
    // Save file location
    private static string savePath = Application.persistentDataPath + "/save.json";

    /// <summary>
    /// Save game function
    /// </summary>
    public static void SaveGame(SaveData data)
    {
        // Convert the SaveData object into a JSON string.
        string json = JsonUtility.ToJson(data, true);

        // Write the JSON string into a file at 'savePath'.
        // If the file doesn’t exist, it will be created. If it does exist, it will be overwritten.
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved at: " + savePath);
    }

    /// <summary>
    /// If there is save file, load game, if not create a new save.
    /// </summary>
    public static SaveData LoadGame()
    {
        // Check if the save file exists before trying to load it.
        if (File.Exists(savePath))
        {
            // Read all text content from the file.
            string json = File.ReadAllText(savePath);

            // Convert the JSON string back into a SaveData object.
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Game loaded from : " + savePath);
            return data;
        }
        Debug.Log("No save data found. Creating new save.");

        // Default value
        return new SaveData() 
        { 
            currentLevel = 1, 
            currentPhase = GamePhase.Farm
        };
    }

    /// <summary>
    /// Check whether you have a save file or not
    /// </summary>
    public static bool HasSaveData()
    {
        return File.Exists(savePath);
    }

    /// <summary>
    /// Clear save file (idk maybe you want to create new save file)
    /// </summary>
    public static void ClearSave()
    {
        // If a save file exists, delete it.
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted: " + savePath);
        }
    }   
}
