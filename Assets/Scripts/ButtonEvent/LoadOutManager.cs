using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class LoadOutManager : MonoBehaviour
{
    [SerializeField] private Transform loadoutPanel;
    [SerializeField] private GameObject loadoutSlotPrefab;

    private List<GameObject> currentLoadout = new List<GameObject>();

    public void AddToLoadout(string itemName)
    {
        // Optional: limit number of loadout items (e.g., 4)
        if (currentLoadout.Count >= 4)
        {
            Debug.Log("Loadout full!");
            return;
        }

        // Create new slot button
        GameObject slot = Instantiate(loadoutSlotPrefab, loadoutPanel);
        slot.GetComponentInChildren<TextMeshProUGUI>().text = itemName;

        // Add remove functionality
        Button btn = slot.GetComponent<Button>();
        btn.onClick.AddListener(() => RemoveFromLoadout(slot));

        currentLoadout.Add(slot);
    }

    public void RemoveFromLoadout(GameObject slot)
    {
        currentLoadout.Remove(slot);
        Destroy(slot);
    }
}
