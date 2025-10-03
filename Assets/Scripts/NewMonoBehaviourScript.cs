using UnityEngine;
using UnityEditor;
using System.IO;

public class CleanupUIToolkit : EditorWindow
{
    [MenuItem("Tools/Scan for UI Toolkit Files")]
    public static void ScanUIToolkit()
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        int foundCount = 0;

        foreach (string path in assetPaths)
        {
            // Check for UXML or USS files
            if (path.EndsWith(".uxml") || path.EndsWith(".uss"))
            {
                Debug.Log($"[UIToolkit Asset] {path}");
                foundCount++;
            }

            // Check for scripts referencing UI Toolkit
            if (path.EndsWith(".cs"))
            {
                string scriptContent = File.ReadAllText(path);
                if (scriptContent.Contains("UnityEngine.UIElements") ||
                    scriptContent.Contains("UnityEditor.UIElements"))
                {
                    Debug.Log($"[UIToolkit Script] {path}");
                    foundCount++;
                }
            }
        }

        if (foundCount == 0)
        {
            Debug.Log("✅ No UI Toolkit assets or scripts found in your project.");
        }
        else
        {
            Debug.LogWarning($"⚠️ Found {foundCount} UI Toolkit-related files. Check the log above to delete them safely.");
        }
    }
}