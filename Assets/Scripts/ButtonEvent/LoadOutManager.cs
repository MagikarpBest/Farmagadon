using Farm;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadOutManager : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private int loadoutColumn = 2;
    [SerializeField] private int bagColumn = 4;
    [SerializeField] private List<Button> loadoutSlots;
    [SerializeField] private List<Button> bagButtons;
    private int selectedIndex = 0;
    private int selectedBagIndex = 0;
    private bool selectingBag = false;
    private bool slotActive = false;
    private bool isColumn = false;
    //list for slots
    Dictionary<int, GameObject> slotItems = new Dictionary<int, GameObject>();
    private List<GameObject> currentLoadout = new List<GameObject>();

    private float moveCooldown = 0.1f;
    private float lastMoveTime = 0f;

    private void Start()
    {
        for(int i = 0; i < loadoutSlots.Count; i++)
        {
            Button button = loadoutSlots[i];
            Outline outline = button.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
        for (int i = 0; i < bagButtons.Count; i++)
        {
            Button button = bagButtons[i];
            Outline outline = button.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
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
        MovementInput();
    }

    public void MovementInput()
    {
        if (gameInput == null) return;

        Vector2 moveInput = gameInput.GetMovementVectorNormalized();

        //horizontal
        if (Mathf.Abs(moveInput.x) > 0.5f && Time.time - lastMoveTime > moveCooldown)
        {
            isColumn = false;
            if (moveInput.x > 0)
                MoveSelection(1); // D
            else
                MoveSelection(-1); // A

            lastMoveTime = Time.time;
        }

        if (Mathf.Abs(moveInput.y) > 0.5f && Time.time - lastMoveTime > moveCooldown)
        {
            isColumn = true;
            if (selectingBag)
            {
                if (moveInput.y > 0)
                    MoveSelection(-bagColumn); // W
                else
                    MoveSelection(bagColumn); // S
            }
            else
            {
                if (moveInput.y > 0)
                    MoveSelection(-loadoutColumn); // W
                else
                    MoveSelection(loadoutColumn); // S
            }

            lastMoveTime = Time.time;
        }
    }

    private void MoveSelection(int direction)
    {

        if (!selectingBag)
        {
            if (loadoutSlots.Count == 0) return;
            
            int nextIndex = selectedIndex + direction;

            //just stop moving when it reach the edge
            // up down

            if (isColumn)
            {
                if (nextIndex < 0 || nextIndex >= loadoutSlots.Count) return;
            }
            else //left right
            {
                int currentRow = selectedIndex / loadoutColumn;
                int nextRow = nextIndex / loadoutColumn;
                if (nextRow != currentRow || nextIndex < 0 || nextIndex >= loadoutSlots.Count) return;
            }

            HighlightSlot(selectedIndex, false);
            selectedIndex = nextIndex;
            HighlightSlot(selectedIndex, true);
            Debug.Log($"Selected loadout slot {selectedIndex}");
        }
        else
        {
            //move at inventory
            if (bagButtons.Count == 0) return;

            //HighlightBag(selectedBagIndex, false);
            int nextBagIndex = selectedBagIndex + direction;

            if (isColumn) //up down
            {
                if (nextBagIndex < 0 || nextBagIndex >= bagButtons.Count) return;
            }
            else //left right
            {
                int currentRow = selectedBagIndex / bagColumn;
                int nextRow = nextBagIndex / bagColumn;
                if (nextRow != currentRow || nextBagIndex < 0 || nextBagIndex >= bagButtons.Count) return;
            }

            
            HighlightBag(selectedBagIndex, false);
            selectedBagIndex = nextBagIndex;
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

        Outline outline = button.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = highlight;
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

        Button button = bagButtons[index];
        HighlightButton(bagButtons[index], highlight);
        DescriptionManager descManager = button.GetComponent<DescriptionManager>();
        if (descManager != null)
        {
            descManager.ShowDescription();
        }
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