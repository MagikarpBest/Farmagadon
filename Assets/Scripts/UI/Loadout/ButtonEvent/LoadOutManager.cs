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
    [SerializeField] private List<Button> loadoutSlots;
    [SerializeField] private List<Button> bagButtons;
    [SerializeField] private List<TextMeshProUGUI> ammoText; 
    [SerializeField] private Button battleButton;
    [SerializeField] private CraftManager craftManager;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    //[SerializeField] private WeaponUI weaponUI;
    //[SerializeField] private AmmoUI ammoUI;
    [SerializeField] private GameObject prefabLoadoutPopup;
    [SerializeField] private GameObject prefabCraftingPopup;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private List<Image> slotImage;
    [SerializeField] private List<Image> bagImage;

    [SerializeField] private AudioClip loadoutBGM;

    private int selectedIndex = 0;
    private int selectedBagIndex = 0;
    private bool selectingBag = false;
    private bool slotActive = false;
    //movement check
    private bool isColumn = false;
    //popup
    private List<Button> popupButtons;
    private int popupIndex = 0;
    private GameObject popup;
    private GameObject craftPopup;
    private bool isPopupActive = false;
    private bool isCraft = false;

    private WeaponSlot[] weapons;



    //lists for both slots
    Dictionary<int, GameObject> slotItems = new Dictionary<int, GameObject>();
    private List<GameObject> currentLoadout = new List<GameObject>();

    //private float moveCooldown = 0.15f;
    //private float lastMoveTime = 0f;

    private IEnumerator Start()
    {
        yield return null;
        for (int i = 0; i < loadoutSlots.Count; i++) 
        {
            slotImage[i].enabled = false;
        }
        AudioService.AudioManager.PlayBGM(loadoutBGM, 1f);
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
        ShowAmmoCount();
    }

    //private void MoveSelection(int direction)
    //{
        

    //    if (!selectingBag)
    //    {
    //        if (loadoutSlots.Count == 0) return;
            
    //        int nextIndex = selectedIndex + direction;

    //        selectedIndex = nextIndex;

    //        Debug.Log($"Selected loadout slot {selectedIndex}");
    //    }
    //    else
    //    {
    //        //move at inventory
    //        if (bagButtons.Count == 0) return;

    //        //HighlightBag(selectedBagIndex, false);
    //        int nextBagIndex = selectedBagIndex + direction;

    //        if (isColumn) //up down
    //        {
    //            if (nextBagIndex < 0 || nextBagIndex >= bagButtons.Count) return;
    //        }
    //        else //left right
    //        {
    //            //Check if there's still any popup and close it
    //            if (isPopupActive && popup != null || craftPopup != null)
    //            {
    //                ClosePopup();
    //                CloseCraftPopup();
    //            }
    //            int currentRow = selectedBagIndex / bagColumn;
    //            int nextRow = nextBagIndex / bagColumn;
    //            if (nextRow != currentRow || nextBagIndex < 0 || nextBagIndex >= bagButtons.Count) return;

                
    //        }

    //        //HighlightBag(selectedBagIndex, false);
    //        selectedBagIndex = nextBagIndex;
    //       // HighlightBag(selectedBagIndex, true);
    //        Debug.Log($"Selected bag item {selectedBagIndex}");
    //    }
    //}

    private void ShowAmmoCount()
    {
        if(weaponInventory == null || ammoInventory == null) return;
        if (ammoText == null || ammoText.Count == 0) return;

        int slotCount = ammoText.Count;
        for (int i = 0; i < ammoText.Count; i++)
        {
            //assign ammocount to each slot text
            TextMeshProUGUI textField = ammoText[i];
            

            if (textField == null) continue;

            //assign slot
            WeaponSlot slot = weaponInventory.GetWeaponSlot(i);

            if (slot != null && slot.weaponData != null && slot.weaponData.ammoType != null)
            {
                int count = ammoInventory.GetAmmoCount(slot.weaponData.ammoType);
                SetImage(bagImage[i],slot);
                textField.text = $"{count}";
            }
            else if(slot==null)
            {
                textField.text = "N/A";
                SetImage(bagImage[i], null);
            }
        }
        
    }

    private void SetImage(Image image, WeaponSlot slot)
    {
        if (image == null)
        {
            return;
        }

        if (slot != null && slot.weaponData != null && slot.weaponData.weaponSprite != null)
        {
            image.sprite = slot.weaponData.weaponSprite;
        }
        else if(slot==null)
        {
            image.sprite = emptySlotSprite;
        }
    }

    
    private void ComfirmSelection()
    {
        GameObject selectedBtn = EventSystem.current.currentSelectedGameObject;
        
        Debug.Log(selectedBagIndex);

        DescriptionManager descManager = selectedBtn.GetComponent<DescriptionManager>();
        if (descManager != null)
        {
            descManager.ShowDescription();
        }
        if (isPopupActive)
        {
            PopupSelect();
            ClosePopup();
            return;
        }

        if (prefabLoadoutPopup != null && !isPopupActive)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            popup = Instantiate(prefabLoadoutPopup, canvas.transform);

            
            RectTransform bagRect = selectedBtn.GetComponent<RectTransform>();
            RectTransform popupRect = popup.GetComponent<RectTransform>();
            popup.transform.SetAsLastSibling();

            popupRect.position = bagRect.position + new Vector3(200f, 0f, 0f);
            isPopupActive = true;
            StartCoroutine(PopupSetup());
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

        EventSystem.current.SetSelectedGameObject(popupButtons[0].gameObject);
        foreach (Button button in popupButtons)
        {
            button.onClick.AddListener(PopupSelect);
        }
    }

    private void PopupSelect()
    {
        Button bagButton = bagButtons[selectedBagIndex];
        WeaponSlot slot = weaponInventory.GetWeaponSlot(selectedBagIndex);
        string itemName = slot.weaponData.weaponID;

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
            AddToLoadout(itemName);
            selectingBag = false;
            Debug.Log("Added");
            Debug.Log(itemName);
            isPopupActive = false;
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
            }
            isCraft = false;
            Debug.Log("Closed craft popup using Pause key");
        }
        EventSystem.current.SetSelectedGameObject(bagButtons[selectedBagIndex].gameObject);
    }

    public void AddToLoadout(string itemName)
    {
        // prevent duplicates based on sprite instead of text
        GameObject selectedBtn = EventSystem.current.currentSelectedGameObject;
        Image bagIcon = selectedBtn.GetComponentInChildren<Image>();

        for (int i = 0; i < loadoutSlots.Count; i++)
        {
            Image checkIcon = loadoutSlots[i].GetComponentInChildren<Image>();
            if (checkIcon != null && checkIcon.sprite == bagIcon.sprite)
                return; // already in loadout
        }

        //if (!slotActive)
        //{
        //    Debug.Log("No loadout slot selected");
        //    return;
        //}

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

    private void RemoveFromLoadout()
    {
        
    }
}