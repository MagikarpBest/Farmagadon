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

    public void AddToLoadout(string itemName)
    {
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
