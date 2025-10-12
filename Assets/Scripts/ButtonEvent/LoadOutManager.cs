using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class LoadOutManager : MonoBehaviour
{
    [SerializeField] private RectTransform loadoutPanel;
    [SerializeField] private GameObject loadoutSlotPrefab;

    private List<GameObject> currentLoadout = new List<GameObject>();


    private Color normalColor = Color.white;
    private Color selectedColor = Color.yellow;

    public void SelectSlot(Button slot)
    {
        // Deselect previous
        if (selectedSlot != null)
            SetSlotColor(selectedSlot, normalColor);

        // Select new
        selectedSlot = slot;
        SetSlotColor(slot, selectedColor);
        Debug.Log($"Selected slot: {slot.name}");
    }


    public void AddToLoadout(string itemName)
    {
        if (selectedSlot == null)
        {
            Debug.Log("No loadout slot selected!");
            return;
        }

        // Prevent double assignment
        if (slotToItem.ContainsKey(selectedSlot))
        {
            Debug.Log("Slot already occupied!");
            return;
        }
        
        // limit number of loadout items (e.g., 4)
        if (currentLoadout.Count >= 4)
        {
            Debug.Log("full");
            return;
        }

        GameObject buttonObj = EventSystem.current.currentSelectedGameObject;
        if (buttonObj != null)
        {
            Button bagButton = buttonObj.GetComponent<Button>();
            if (bagButton != null)
            {
                bagButton.interactable = false; // disable the inventory button
            }
        }

        // Create new slot button
        GameObject slot = Instantiate(loadoutSlotPrefab, loadoutPanel);
        Debug.Log($"Added {itemName} to {slot.transform.parent.name}");
        //StartCoroutine(RefreshLayout());
        //slot.transform.SetParent(loadoutPanel, false);


        TextMeshProUGUI text = slot.GetComponentInChildren<TextMeshProUGUI>(true);
        text.text = itemName;

        //Make sure prefab or its Button has Button component
        Button slotButton = slot.GetComponentInChildren<Button>();
        slotButton.onClick.AddListener(() => RemoveFromLoadout(slot, buttonObj));

        currentLoadout.Add(slot);
        Debug.Log(currentLoadout.Count);
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

    private IEnumerator RefreshLayout()
    {
        yield return null; // wait 1 frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(loadoutPanel.GetComponent<RectTransform>());
    }
}
