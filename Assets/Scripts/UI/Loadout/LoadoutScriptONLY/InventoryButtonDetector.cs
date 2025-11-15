using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButtonDetector : MonoBehaviour,ISelectHandler
{
    private LoadOutManager loadOutManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        // Automatically find the LoadOutManager in the scene
        if (loadOutManager == null)
        {
            loadOutManager = FindFirstObjectByType<LoadOutManager>();
            if (loadOutManager == null)
            {
                Debug.LogError("No LoadOutManager found in the scene!");
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (loadOutManager != null)
        {
            loadOutManager.OnSlotSelected(button);
        }
    }
}
