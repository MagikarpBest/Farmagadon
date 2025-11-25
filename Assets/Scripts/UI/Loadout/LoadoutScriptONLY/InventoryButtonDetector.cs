using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This is for loadout manager to know which button is selected and update visual based on what return.
/// Do not remove just because you see no references!!!!
/// </summary>
public class InventoryButtonDetector : MonoBehaviour,ISelectHandler
{
    private LoadoutManager loadoutManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        // Automatically find the LoadOutManager in the scene
        if (loadoutManager == null)
        {
            loadoutManager = FindFirstObjectByType<LoadoutManager>();
            if (loadoutManager == null)
            {
                Debug.LogError("No LoadOutManager found in the scene!");
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (loadoutManager != null)
        {
            loadoutManager.OnSlotSelected(button);
        }
    }
}
