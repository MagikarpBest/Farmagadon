using UnityEngine;
using UnityEngine.EventSystems;
using System.Diagnostics;
using System.Text;

public class FullSelectionTracker : MonoBehaviour
{
    private GameObject lastSelected;

    void Update()
    {
        var current = EventSystem.current.currentSelectedGameObject;

        if (current != lastSelected)
        {
            lastSelected = current;

            // Log new selection
            LogSelectionChange(current);
        }
    }

    private void LogSelectionChange(GameObject current)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Selection changed! New: {(current != null ? current.name : "null")}");
        sb.AppendLine("Call stack:");

        // Get full stack trace
        StackTrace stackTrace = new StackTrace(true);

        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            var method = frame.GetMethod();
            if (method == null) continue;

            sb.AppendLine($"{method.DeclaringType}.{method.Name} (Line {frame.GetFileLineNumber()})");
        }

        UnityEngine.Debug.Log(sb.ToString());
    }
}