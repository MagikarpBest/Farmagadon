using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LoadOutManager : MonoBehaviour
{
    [SerializeField] private List<Button> loadoutSlots;
    [SerializeField] private GameObject loadoutSlotPrefab;

    private int selectedIndex = 0;
    private bool slotActive = false;
    //list for slots
    Dictionary<int, GameObject> slotItems = new Dictionary<int, GameObject>();
    private List<GameObject> currentLoadout = new List<GameObject>();


    //private Color normalColor = Color.white;
    //private Color selectedColor = Color.yellow;

    public void AddToLoadout(string itemName)
    {
        //debug
        if (!slotActive)
        {
            Debug.Log("No loadout slot selected");
            return;
        }

        int index=selectedIndex;

        // Prevent double assignment
        if (slotItems.ContainsKey(index))
        {
            Debug.Log("Slot already occupied");
            return;
        }

        //// limit number of loadout items (e.g., 4)
        //if (currentLoadout.Count >= 4)
        //{
        //    Debug.Log("full");
        //    return;

        Button targetSlot = loadoutSlots[index];
        Transform slotTransform = targetSlot.transform;

        // Create new slot button
        //var slot = loadoutSlots;
        GameObject icon = Instantiate(loadoutSlotPrefab, slotTransform);

        TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>(true);
        text.text = itemName;

        slotItems[index] = icon;

        currentLoadout.Add(icon);
        Debug.Log(currentLoadout.Count);
        slotActive = false;
    }

    public void RemoveFromLoadout(GameObject slot, GameObject buttonObj)
    {
        currentLoadout.Remove(slot);
        Destroy(slot);

        if (buttonObj != null)
        {
            Button bagButton = buttonObj.GetComponent<Button>();
            if (bagButton != null)
            {
                bagButton.interactable = true;
            }
        }
    }


    /*private IEnumerator RefreshLayout()
    {
        yield return null; // wait 1 frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(loadoutPanel.GetComponent<RectTransform>());
    }*/
}
