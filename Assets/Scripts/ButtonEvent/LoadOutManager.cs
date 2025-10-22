using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using System.Xml.Serialization;

public class LoadOutManager : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private List<Button> loadoutSlots;
    [SerializeField] private GameObject loadoutSlotPrefab;

    private int selectedIndex = 0;
    private bool slotActive = false;
    //list for slots
    Dictionary<int, GameObject> slotItems = new Dictionary<int, GameObject>();
    private List<GameObject> currentLoadout = new List<GameObject>();

    private float moveCooldown = 0.2f;
    private float lastMoveTime = 0f;

    private void Start()
    {
        //highlight first slot
        HighlightSlot(selectedIndex, true);
    }

    private void OnEnable()
    {
        if(gameInput != null)
        {
            gameInput.OnShootAction += ComfirmSelection;
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnShootAction -= ComfirmSelection;
        }
    }

    private void Update()
    {
        HandleKeyboardNavigation();
    }

    private void HandleKeyboardNavigation()
    {
        if (gameInput == null) return;

        Vector2 moveInput = gameInput.GetMovementVectorNormalized();

        if (Mathf.Abs(moveInput.x) > 0.5f && Time.time - lastMoveTime > moveCooldown)
        {
            if (moveInput.x > 0)
                MoveSelection(1); // D / Right
            else
                MoveSelection(-1); // A / Left

            lastMoveTime = Time.time;
        }
    }

    private void MoveSelection(int direction)
    {
        if (loadoutSlots.Count == 0) return;

        // Deselect current
        HighlightSlot(selectedIndex, false);

        // Move index
        selectedIndex += direction;
        if (selectedIndex < 0) selectedIndex = loadoutSlots.Count - 1;
        else if (selectedIndex >= loadoutSlots.Count) selectedIndex = 0;

        // Select new
        HighlightSlot(selectedIndex, true);
        slotActive = true;

        Debug.Log($"Selected slot: {selectedIndex}");
    }

    private void ComfirmSelection()
    {
        if(!slotActive) return;

        Debug.Log("Added");
        AddToLoadout("Test");
    }

    private void HighlightSlot(int index, bool highlight)
    {
        if (index < 0 || index >= loadoutSlots.Count) return;
        Image img = loadoutSlots[index].GetComponent<Image>();

        if (img != null)
            img.color = highlight ? Color.yellow : Color.white;
    }

    public void AddToLoadout(string itemName)
    {
        //debug
        if (!slotActive)
        {
            Debug.Log("No loadout slot selected");
            return;
        }

        int index = selectedIndex;

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
        Debug.Log("created");
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

}