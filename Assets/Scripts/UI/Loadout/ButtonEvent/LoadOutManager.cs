using Farm;
using System;
using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] private Button battleButton;
    [SerializeField] private GameObject prefabLoadoutPopup;
    [SerializeField] private GameObject prefabCraftingPopup;

    private int selectedIndex = 0;
    private int selectedBagIndex = 0;
    private bool selectingBag = false;
    private bool slotActive = false;
    //movement check
    private bool isColumn = false;
    //battle button
    private bool isBattle = false;
    //popup
    private List<Button> popupButtons;
    private int popupIndex = 0;
    private GameObject popup;
    private GameObject craftPopup;
    private bool isPopupActive = false;
    private bool isCraft = false;

    //lists for both slots
    Dictionary<int, GameObject> slotItems = new Dictionary<int, GameObject>();
    private List<GameObject> currentLoadout = new List<GameObject>();

    private float moveCooldown = 0.15f;
    private float lastMoveTime = 0f;

    private void Start()
    {
        //for(int i = 0; i < loadoutSlots.Count; i++)
        //{
        //    Button button = loadoutSlots[i];
        //    Outline outline = button.GetComponent<Outline>();
        //    if (outline != null)
        //        outline.enabled = false;
        //}
        //for (int i = 0; i < bagButtons.Count; i++)
        //{
        //    Button button = bagButtons[i];
        //    Outline outline = button.GetComponent<Outline>();
        //    if (outline != null)
        //        outline.enabled = false;
        //}
        //Outline outlineBattle = battleButton.GetComponent<Outline>();
        //if (outlineBattle != null)
        //    outlineBattle.enabled = false;
        ////highlight first slot
        //HighlightSlot(selectedIndex, true);
    }

    private void OnEnable()
    {
        if(gameInput != null)
        {
            gameInput.OnShootAction += ComfirmSelection;
            gameInput.OnPause += CloseCraftPopup;
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnShootAction -= ComfirmSelection;
            gameInput.OnPause -= CloseCraftPopup;
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
        if (!isPopupActive && Mathf.Abs(moveInput.x) > 0.5f && Time.time - lastMoveTime > moveCooldown)
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
            if (isPopupActive)
            {
                if (moveInput.y > 0)
                    MoveSelection(-1); // W
                else
                    MoveSelection(1); // S
            }
            else if (selectingBag)
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
        if (isPopupActive && popupButtons.Count > 0 && isColumn)
        {
            int nextIndex = popupIndex + direction;
            if (nextIndex < 0 || nextIndex >= popupButtons.Count) return;
            //HighlightButton(popupButtons[popupIndex], false);
            popupIndex = nextIndex;
            
            Debug.Log(popupIndex);
            //HighlightButton(popupButtons[popupIndex], true);
            return;
        }

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

            //HighlightSlot(selectedIndex, false);
            selectedIndex = nextIndex;
            //HighlightSlot(selectedIndex, true);
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
                //Check if there's still any popup and close it
                if (isPopupActive && popup != null || craftPopup != null)
                {
                    ClosePopup();
                    CloseCraftPopup();
                }
                int currentRow = selectedBagIndex / bagColumn;
                int nextRow = nextBagIndex / bagColumn;
                if (nextRow != currentRow || nextBagIndex < 0 || nextBagIndex >= bagButtons.Count) return;

                
            }

            //HighlightBag(selectedBagIndex, false);
            selectedBagIndex = nextBagIndex;
           // HighlightBag(selectedBagIndex, true);
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
            //UpdateHighlights();

            return;
        }

        if (slotActive && selectingBag)
        {
            Button bagButton = bagButtons[selectedBagIndex];
            
            if (isPopupActive)
            {
                PopupSelect();
                ClosePopup();
                return;
            }
            if (prefabLoadoutPopup != null && !isPopupActive)
            {
                //get canvas and spawn under canvas as last object, so it wont be behind buttons
                Canvas canvas = FindAnyObjectByType<Canvas>();

                popup = Instantiate(prefabLoadoutPopup, canvas.transform);

                //set position
                RectTransform bagRect = bagButtons[selectedBagIndex].GetComponent<RectTransform>();
                RectTransform popupRect = popup.GetComponent<RectTransform>();
                popup.transform.SetAsLastSibling();
                Debug.Log(bagRect.position);
                popupRect.position = bagRect.position + new Vector3(180f, 0f, 0f);

                isPopupActive = true;
                StartCoroutine(PopupSetup());
            }
            //UpdateHighlights();
        }

        
        Debug.Log(currentLoadout.Count);
    }

    private void ClosePopup()
    {
        if (popup != null && !isCraft)
        {
            Destroy(popup);
        }
        isPopupActive = false;

        popupButtons.Clear();
        popupIndex = 0;
    }

    private IEnumerator PopupSetup()
    {
        yield return null;

        if (popupButtons == null || popupButtons.Count == 0)
        {
            popupButtons = new List<Button>(popup.GetComponentsInChildren<Button>(true));
        }
        if (popupIndex < 0 || popupIndex >= popupButtons.Count)
        {
            popupIndex = 0;
        }

        foreach (Button button in popupButtons)
        {
            Outline outline = button.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
        HighlightButton(popupButtons[popupIndex], true);
    }

    private void PopupSelect()
    {
        Button bagButton = bagButtons[selectedBagIndex];
        TextMeshProUGUI textComponent = bagButton.GetComponentInChildren<TextMeshProUGUI>();

        if (popupIndex == 1 && !isCraft)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();

            craftPopup = Instantiate(prefabCraftingPopup, canvas.transform);

            //set position
            RectTransform bagRect = bagButtons[selectedBagIndex].GetComponent<RectTransform>();
            RectTransform craftRect = craftPopup.GetComponent<RectTransform>();
            craftPopup.transform.SetAsLastSibling();
            Debug.Log(bagRect.position);
            craftRect.position = bagRect.position + new Vector3(180f, 0f, 0f);
            isCraft = true;
            isPopupActive = false;
        }
        else if (isPopupActive && popupIndex==0)
        {
            string itemName = textComponent.text;
            AddToLoadout(itemName);
            selectingBag = false;
            HighlightButton(bagButtons[selectedBagIndex], false);
            Debug.Log("Added");
            isPopupActive = false;
            UpdateHighlights ();
        }
    }

    private void CloseCraftPopup()
    {
        if (craftPopup != null)
        {
            Destroy(craftPopup);
            craftPopup = null;
            if (popup != null)
            {
                isPopupActive = true;
                StartCoroutine(PopupSetup());
                Debug.Log(popupIndex);
                HighlightButton(popupButtons[popupIndex], true);
            }
            isCraft = false;
            Debug.Log("Closed craft popup using Pause key");
        }
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
        //Highlight active place
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
        // prevent duplicates based on sprite instead of text
        Button bagButton = bagButtons[selectedBagIndex];
        Image bagIcon = bagButton.GetComponentInChildren<Image>();

        for (int i = 0; i < loadoutSlots.Count; i++)
        {
            Image checkIcon = loadoutSlots[i].GetComponentInChildren<Image>();
            if (checkIcon != null && checkIcon.sprite == bagIcon.sprite)
                return; // already in loadout
        }

        if (!slotActive)
        {
            Debug.Log("No loadout slot selected");
            return;
        }

        int index = selectedIndex;
        Button targetSlot = loadoutSlots[index];
        Image slotIcon = targetSlot.GetComponentInChildren<Image>();

        if (slotIcon != null)
        {
            slotIcon.sprite = bagIcon.sprite;
        }

        if (currentLoadout.Count <= index)
        {
            currentLoadout.Add(targetSlot.gameObject);
        }
        else
        {
            currentLoadout[index] = targetSlot.gameObject;
        }
    }
}