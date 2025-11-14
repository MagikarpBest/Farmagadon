//using System.Collections.Generic;
//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class LoadOutManager : MonoBehaviour
//{
//    [Header("Reference")]
//    [SerializeField] private GameInput gameInput;
//    [SerializeField] private CraftManager craftManager;
//    [SerializeField] private WeaponInventory weaponInventory;
//    [SerializeField] private AmmoInventory ammoInventory;

//    [SerializeField] private Button battleButton;


//    [Header("Inventory Slots Reference")]
//    [SerializeField] private List<Button> loadoutSlots; 
//    [SerializeField] private List<Button> bagButtons;
//    [SerializeField] private List<TextMeshProUGUI> ammoText;
//    [SerializeField] private List<Image> slotImage;
//    [SerializeField] private List<Image> bagImage;
//    [SerializeField] private Sprite emptySlotSprite;

//    [Header("Popup Prefabs")]
//    [SerializeField] private GameObject prefabLoadoutPopup;
//    [SerializeField] private GameObject prefabCraftingPopup;

//    private int selectedIndex = 0;
//    private int selectedBagIndex = 0;
//    private bool selectingBag = false;
//    private bool slotActive = false;
//    //popup
//    private List<Button> popupButtons;
//    private int popupIndex = 0;
//    private GameObject popup;
//    private GameObject craftPopup;
//    private bool isPopupActive = false;
//    private bool isEquipped = false;
//    private bool isCraft = false;

//    private WeaponSlot[] weapons;



//    //lists for both slots
//    Dictionary<int, GameObject> slotItems = new Dictionary<int, GameObject>();
//    private List<GameObject> currentLoadout = new List<GameObject>();
//    private Dictionary<Button, bool> equippedStates = new Dictionary<Button, bool>();

//    private IEnumerator Start()
//    {
//        yield return null;
//        for (int i = 0; i < loadoutSlots.Count; i++) 
//        {
//            slotImage[i].enabled = false;
//        }
//        for (int i = 0; i < bagButtons.Count; i++)
//        {
//            int index = i; // capture loop variable
//            bagButtons[i].onClick.AddListener(() => OnBagSelected(index));
//        }
//        foreach (Button btn in bagButtons)
//        {
//            equippedStates[btn] = false; // not equipped initially
//            btn.onClick.AddListener(() => ToggleEquip(btn));
//        }
//    }
//    private void OnBagSelected(int index)
//    {
//        selectedBagIndex = index;
//        Debug.Log($"Selected bag slot {index}");
//    }
//    void ToggleEquip(Button btn)
//    {
//        // Toggle equipped state
//        equippedStates[btn] = !equippedStates[btn];

//        // Update visuals
//        btn.GetComponent<Image>().color = equippedStates[btn] ? Color.green : Color.white;

//        Debug.Log($"{btn.name} equipped: {equippedStates[btn]}");
//    }

//    private void Update()
//    {
//        GameObject current = EventSystem.current.currentSelectedGameObject;
//        if (current != null)
//        {
//            int index = bagButtons.IndexOf(current.GetComponent<Button>());
//            if (index >= 0 && index != selectedBagIndex)
//            {
//                selectedBagIndex = index;
//                Debug.Log($"Selected bag slot {selectedBagIndex}");
//            }
//        }

//        ShowAmmoCount();
//    }

//    private void OnEnable()
//    {
//        if(gameInput != null)
//        {
//            gameInput.OnShootAction += ComfirmSelection;
//            gameInput.OnPause += CloseCraftPopup;
//        }
//    }

//    private void OnDisable()
//    {
//        if (gameInput != null)
//        {
//            gameInput.OnShootAction -= ComfirmSelection;
//            gameInput.OnPause -= CloseCraftPopup;
//        }
//    }

//    private void ShowAmmoCount()
//    {
//        if(weaponInventory == null || ammoInventory == null) return;
//        if (ammoText == null || ammoText.Count == 0) return;

//        int slotCount = ammoText.Count;
//        for (int i = 0; i < ammoText.Count; i++)
//        {
//            //assign ammocount to each slot text
//            TextMeshProUGUI textField = ammoText[i];
            

//            if (textField == null) continue;

//            //assign slot
//            WeaponSlot slot = weaponInventory.GetWeaponSlot(i);

//            if (slot != null && slot.weaponData != null && slot.weaponData.ammoType != null)
//            {
//                int count = ammoInventory.GetAmmoCount(slot.weaponData.ammoType);
//                SetImage(bagImage[i],slot);
//                textField.text = $"{count}";
//            }
//            else if(slot==null)
//            {
//                textField.text = "N/A";
//                SetImage(bagImage[i], null);
//            }
//        }
        
//    }

//    private void SetImage(Image image, WeaponSlot slot)
//    {
//        if (image == null)
//        {
//            return;
//        }

//        if (slot != null && slot.weaponData != null && slot.weaponData.weaponSprite != null)
//        {
//            image.sprite = slot.weaponData.weaponSprite;
//        }
//        else if(slot==null)
//        {
//            image.sprite = emptySlotSprite;
//        }
//    }
    
//    private void ComfirmSelection()
//    {
//        GameObject selectedBtn = EventSystem.current.currentSelectedGameObject;
//        Debug.Log(selectedBagIndex);
//        Debug.Log(equippedStates[bagButtons[selectedBagIndex]]);
//        DescriptionManager descManager = selectedBtn.GetComponent<DescriptionManager>();
//        if (descManager != null)
//        {
//            descManager.ShowDescription();
//        }
//        if (isPopupActive)
//        {
//            PopupSelect();
//            ClosePopup();
//            EventSystem.current.SetSelectedGameObject(bagButtons[selectedBagIndex].gameObject);
//            return;
//        }

//        if (prefabLoadoutPopup != null && !isPopupActive)
//        {
//            Canvas canvas = FindAnyObjectByType<Canvas>();
//            popup = Instantiate(prefabLoadoutPopup, canvas.transform);

            
//            RectTransform bagRect = selectedBtn.GetComponent<RectTransform>();
//            RectTransform popupRect = popup.GetComponent<RectTransform>();
//            popup.transform.SetAsLastSibling();

//            popupRect.position = bagRect.position + new Vector3(200f, 0f, 0f);
//            isPopupActive = true;
//            StartCoroutine(PopupSetup());
//        }
//        Debug.Log(currentLoadout.Count);
//    }

//    private void ClosePopup()
//    {
//        if (popup != null && !isCraft)
//        {
//            Destroy(popup);
//        }
//        isPopupActive = false;

//        popupButtons.Clear();
//        popupIndex = 0;
//    }

//    private IEnumerator PopupSetup()
//    {
//        yield return null;

//        if (popupButtons == null || popupButtons.Count == 0)
//        {
//            popupButtons = new List<Button>(popup.GetComponentsInChildren<Button>(true));
//        }

//        EventSystem.current.SetSelectedGameObject(popupButtons[0].gameObject);
//        foreach (Button button in popupButtons)
//        {
//            button.onClick.AddListener(PopupSelect);
//        }
//    }

//    private void PopupSelect()
//    {
//        //Button bagButton = bagButtons[selectedBagIndex];
//        int bagIndex = selectedBagIndex;
//        WeaponSlot slot = weaponInventory.GetWeaponSlot(selectedBagIndex);
//        string itemName = slot.weaponData.weaponID;

//        if (popupIndex == 1 && !isCraft)
//        {
//            Canvas canvas = FindAnyObjectByType<Canvas>();

//            craftPopup = Instantiate(prefabCraftingPopup, canvas.transform);

//            //set position
//            RectTransform bagRect = bagButtons[selectedBagIndex].GetComponent<RectTransform>();
//            RectTransform craftRect = craftPopup.GetComponent<RectTransform>();
//            craftPopup.transform.SetAsLastSibling();
//            Debug.Log(bagRect.position);
//            craftRect.position = bagRect.position + new Vector3(180f, 0f, 0f);
//            isCraft = true;
//            isPopupActive = false;
//        }
//        else if (isPopupActive && popupIndex==0)
//        {
//            if (!equippedStates[bagButtons[selectedBagIndex]])
//            {
//                AddToLoadout(itemName);
//                equippedStates[bagButtons[selectedBagIndex]] = true;
//                Debug.Log("Added");
//                Debug.Log(itemName);
//            }
//            else
//            {
//                RemoveFromLoadout(itemName);
//                equippedStates[bagButtons[selectedBagIndex]] = false;
//                Debug.Log("Removed");
//                Debug.Log(itemName);
//            }
//            isPopupActive = false;
//        }
        
//    }

//    private void CloseCraftPopup()
//    {
//        if (craftPopup != null)
//        {
//            Destroy(craftPopup);
//            craftPopup = null;
//            if (popup != null)
//            {
//                isPopupActive = true;
//                StartCoroutine(PopupSetup());
//                Debug.Log(popupIndex);
//            }
//            isCraft = false;
//            Debug.Log("Closed craft popup using J");
//        }
//        EventSystem.current.SetSelectedGameObject(bagButtons[selectedBagIndex].gameObject);
//    }

//    public void AddToLoadout(string itemName)
//    {
//        // prevent duplicates based on sprite instead of text
//        GameObject selectedBtn = EventSystem.current.currentSelectedGameObject;
//        Image bagIcon = selectedBtn.GetComponentInChildren<Image>();

//        for (int i = 0; i < loadoutSlots.Count; i++)
//        {
//            Image checkIcon = slotImage[i];
//            if (checkIcon != null && checkIcon.sprite == bagIcon.sprite)
//                return; // already in loadout
//        }


//        int index = selectedIndex;
//        Button targetSlot = loadoutSlots[index];
//        Image slotIcon = targetSlot.GetComponentInChildren<Image>();

//        if (slotIcon != null)
//        {
//            slotIcon.sprite = bagIcon.sprite;
//        }

//        if (currentLoadout.Count <= index)
//        {
//            currentLoadout.Add(targetSlot.gameObject);
//        }
//        else
//        {
//            currentLoadout[index] = targetSlot.gameObject;
//        }
//    }

//    private void RemoveFromLoadout(string itemName)
//    {
//        int index = selectedIndex;
//        Button targetSlot = loadoutSlots[index];
//        Image slotIcon = targetSlot.GetComponentInChildren<Image>();
//        if (currentLoadout.Count <= index)
//        {
//            currentLoadout.Remove(targetSlot.gameObject);
//        }
//        else
//        {
//            currentLoadout[index] = null;
//        }
//    }
//}