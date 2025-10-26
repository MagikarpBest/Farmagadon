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
    [SerializeField] private List<Button> bagButtons;

    private int selectedIndex = 0;
    private int selectedBagIndex = 0;
    private bool selectingBag = false;
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
        if (!selectingBag)
        {
            // Move between loadout slots
            if (loadoutSlots.Count == 0) return;

            HighlightSlot(selectedIndex, false);
            selectedIndex += direction;

            if (selectedIndex < 0) selectedIndex = loadoutSlots.Count - 1;
            else if (selectedIndex >= loadoutSlots.Count) selectedIndex = 0;

            HighlightSlot(selectedIndex, true);
            Debug.Log($"Selected loadout slot {selectedIndex}");
        }
        else
        {
            // Move between bag items
            if (bagButtons.Count == 0) return;

            HighlightBag(selectedBagIndex, false);
            selectedBagIndex += direction;

            if (selectedBagIndex < 0) selectedBagIndex = bagButtons.Count - 1;
            else if (selectedBagIndex >= bagButtons.Count) selectedBagIndex = 0;

            HighlightBag(selectedBagIndex, true);
            Debug.Log($"Selected bag item {selectedBagIndex}");
        }
    }

    private void ComfirmSelection()
    {
        if (!selectingBag)
        {
            slotActive = true;
            selectingBag = true;

            selectedBagIndex = 0;
            UpdateHighlights();

            return;
        }

        if (slotActive && selectingBag)
        {
            Button bagButton = bagButtons[selectedBagIndex];
            TextMeshProUGUI textComponent = bagButton.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent == null)
            {
                Debug.LogWarning("No text found");
                return;
            }

            string itemName = textComponent.text;

            Debug.Log("Added");

            AddToLoadout(itemName);
            slotActive = false;
            selectingBag = false;
            UpdateHighlights();
        }

        Debug.Log(currentLoadout.Count);
    }

    private void HighlightButton(Button button, bool highlight)
    {
        if (button == null) return;
        Image img = button.GetComponent<Image>();
        if (img != null)
            img.color = highlight ? Color.yellow : Color.white;
    }

    private void UpdateHighlights()
    {
        // Highlight the active row
        HighlightSlot(selectedIndex, !selectingBag);
        HighlightBag(selectedBagIndex, selectingBag);
    }

    private void HighlightSlot(int index, bool highlight)
    {
        if (index < 0 || index >= loadoutSlots.Count) return;
        HighlightButton(loadoutSlots[index], highlight);
    }

    private void HighlightBag(int index, bool highlight)
    {
        if (index < 0 || index >= bagButtons.Count) return;
        HighlightButton(bagButtons[index], highlight);
    }

    public void AddToLoadout(string itemName)
    {
        for (int i = 0; i < loadoutSlots.Count; i++)
        {
            TextMeshProUGUI texts = loadoutSlots[i].GetComponentInChildren<TextMeshProUGUI>();
            if (texts != null && texts.text == itemName)
                return; // already in loadout
        }

        if (!slotActive)
        {
            Debug.Log("No loadout slot selected");
            return;
        }

        int index = selectedIndex;
        Button targetSlot = loadoutSlots[index];
        TextMeshProUGUI slotText = targetSlot.GetComponentInChildren<TextMeshProUGUI>();

        if (slotText != null)
        {
            slotText.text = itemName;
        }

        if (currentLoadout.Count <= index)
            currentLoadout.Add(targetSlot.gameObject);
        else
            currentLoadout[index] = targetSlot.gameObject;
    }
}