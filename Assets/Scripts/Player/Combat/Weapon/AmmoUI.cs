using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;


public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AmmoInventory ammoInventory; 
    [SerializeField] private WeaponInventory weaponInventory;

    [Header("UI Elements (slot order: Center, Right, Bottom, Left)")]
    [SerializeField] private TextMeshProUGUI centerWeaponText;
    [SerializeField] private TextMeshProUGUI rightWeaponText;
    [SerializeField] private TextMeshProUGUI bottomWeaponText;
    [SerializeField] private TextMeshProUGUI leftWeaponText;    // 4th slot 

    [Header("Animation Settings")]
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private Ease rotateEase = Ease.OutQuad;

    // Cached original positions
    private Vector3 leftPosition;
    private Vector3 centerPosition;
    private Vector3 rightPosition;
    private Vector3 bottomPosition;

    // Cached slot sizes for scaling during animation
    private Vector2 originalSize;
    private Vector2 centerBigSize;

    private Action<WeaponSlot, WeaponSwitchDirection> weaponChangedHandler;

    private void OnEnable()
    {
        if(ammoInventory!=null)
        { 
            ammoInventory.OnInventoryChanged += UpdateUI;
        }
        
        if (weaponInventory!=null)  
        {
            // Store the lambda reference to remove later
            weaponInventory.OnWeaponChanged += UpdateUI;
        }
    }
    
    private void OnDisable()
    {
        if (ammoInventory != null)
        {
            ammoInventory.OnInventoryChanged -= UpdateUI;
        }

        if (weaponInventory != null)
        {
            weaponInventory.OnWeaponChanged -= weaponChangedHandler;
        }
    }

    private void Start()
    {
        // Save original position for animation references
        leftPosition = leftWeaponText.rectTransform.anchoredPosition;
        centerPosition = centerWeaponText.rectTransform.anchoredPosition;
        rightPosition = rightWeaponText.rectTransform.anchoredPosition;
        bottomPosition = bottomWeaponText.rectTransform.anchoredPosition;

        // Prevent any stray tween movement
        DOTween.KillAll();

        // Save size
        originalSize = leftWeaponText.rectTransform.sizeDelta;
        centerBigSize = centerWeaponText.rectTransform.sizeDelta;


        if (weaponInventory != null)
        {
            InitializeAmmoText();
        }
    }

    private void InitializeAmmoText()
    {
        // Always show all slots (up to max)
        int unlocked = weaponInventory.UnlockedSlotCount;
        List<WeaponSlot> allSlots = new();

        // Add all unlocked weapon slots
        for (int i = 0; i < unlocked; i++)
        {
            allSlots.Add(weaponInventory.GetWeaponSlot(i)); // may include nulls (locked or empty)
        }

        // If no weapons equipped, display all as empty
        if (allSlots.Count == 0)
        {
            SetEmptyAll();
            return;
        }

        int currentIndex = weaponInventory.GetCurrentWeaponIndex();

        // Wrap around logic using only unlocked slots
        WeaponSlot centerSlot = allSlots[currentIndex];
        WeaponSlot rightSlot = (unlocked > 1) ? allSlots[(currentIndex + 1) % unlocked] : null;
        WeaponSlot bottomSlot = (unlocked > 2) ? allSlots[(currentIndex + 2) % unlocked] : null;
        WeaponSlot leftSlot = (unlocked > 3) ? allSlots[(currentIndex + 3) % unlocked] : null;

        // Assign Text
        SetText(centerWeaponText, centerSlot);
        SetText(rightWeaponText, rightSlot);
        SetText(bottomWeaponText, bottomSlot);
        SetText(leftWeaponText, leftSlot);
    }

    /// <summary>
    /// Updates all ammo slot UI elements to match the player's current weapons and ammo counts.
    /// </summary>
    private void UpdateUI(WeaponSlot weaponSlot, WeaponSwitchDirection direction)
    {
        if (weaponInventory == null || ammoInventory == null) 
        {
            Debug.LogWarning("[AmmoUI] Missing inventory reference.");
            return;
        }

        int unlockedCount = weaponInventory.UnlockedSlotCount;

        if (unlockedCount == 2)
        {
            RotateAnimation_Two();
        }
        else if (unlockedCount == 3)
        {
            RotateAnimation_Three(direction == WeaponSwitchDirection.Next);
        }
        else if (unlockedCount >= 4)
        {
            RotateAnimation_Four(direction == WeaponSwitchDirection.Next);
        }
    }

    /// <summary>
    /// Handles rotation animation when 2 weapons are unlocked.
    /// Simply swaps center and right icons.
    /// </summary>
    private void RotateAnimation_Two()
    {
        var center = centerWeaponText.rectTransform;
        var right = rightWeaponText.rectTransform;

        Sequence seq = DOTween.Sequence();

        seq.Join(center.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
        seq.Join(right.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));

        seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
        seq.Join(right.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));

        seq.OnComplete(() =>
        {
            // Swap references
            TextMeshProUGUI temp = centerWeaponText;
            centerWeaponText = rightWeaponText;
            rightWeaponText = temp;

            ResetPositions();
            ResetSizes();
        });
    }

    /// <summary>
    /// Handles rotation animation when 3 weapons are unlocked.
    /// Cycles between center, right, and bottom positions.
    /// </summary>
    private void RotateAnimation_Three(bool clockwise)
    {
        var center = centerWeaponText.rectTransform;
        var right = rightWeaponText.rectTransform;
        var bottom = bottomWeaponText.rectTransform;

        Sequence seq = DOTween.Sequence();

        // When Q pressed
        if (clockwise)
        {
            // center -> bottom, bottom -> right, right -> center
            seq.Join(center.DOAnchorPos(bottomPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));

            // Adjust sizes 
            seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                // Rotate references
                TextMeshProUGUI temp = centerWeaponText;
                centerWeaponText = rightWeaponText;
                rightWeaponText = bottomWeaponText;
                bottomWeaponText = temp;
                ResetPositions();
                ResetSizes();
            });
        }
        // When E pressed
        else
        {
            // center -> right, right -> bottom, bottom -> center
            seq.Join(center.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOAnchorPos(bottomPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));

            seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                TextMeshProUGUI temp = centerWeaponText;
                centerWeaponText = bottomWeaponText;
                bottomWeaponText = rightWeaponText;
                rightWeaponText = temp;
                ResetPositions();
                ResetSizes();
            });
        }
    }

    /// <summary>
    /// Handles rotation animation when 4 weapons are unlocked.
    /// Fully rotates through all four positions (left, bottom, right, center).
    /// </summary>
    private void RotateAnimation_Four(bool clockwise)
    {
        var left = leftWeaponText.rectTransform;
        var center = centerWeaponText.rectTransform;
        var right = rightWeaponText.rectTransform;
        var bottom = bottomWeaponText.rectTransform;

        Sequence seq = DOTween.Sequence();

        if (clockwise)
        {
            // Anti Clockwise, When E pressed, move icon to left
            // Move positions
            seq.Join(left.DOAnchorPos(bottomPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOAnchorPos(leftPosition, rotateDuration).SetEase(rotateEase));

            // Tween sizes
            seq.Join(left.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                TextMeshProUGUI temp = leftWeaponText;
                leftWeaponText = centerWeaponText;
                centerWeaponText = rightWeaponText;
                rightWeaponText = bottomWeaponText;
                bottomWeaponText = temp;
                ResetPositions();
                ResetSizes();
            });

        }
        else
        {
            // Clockwise, When Q pressed, move icon to right
            // Move positions
            seq.Join(left.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOAnchorPos(bottomPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOAnchorPos(leftPosition, rotateDuration).SetEase(rotateEase));

            // Tween sizes
            seq.Join(left.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                TextMeshProUGUI+ temp = leftWeaponText;
                leftWeaponText = bottomWeaponText;
                bottomWeaponText = rightWeaponText;
                rightWeaponText = centerWeaponText;
                centerWeaponText = temp;
                ResetPositions();
                ResetSizes();
            });
        }
    }

    /// <summary>
    /// Sets text for a specific slot (shows ammo count or N/A)
    /// </summary>
    private void SetText(TextMeshProUGUI textElement, WeaponSlot slot)
    {
        if (textElement == null)
        {
            return;
        }

        if (slot != null && slot.weaponData != null && slot.weaponData.ammoType != null) 
        {
            int count = ammoInventory.GetAmmoCount(slot.weaponData.ammoType);
            textElement.text = $"{count}";
        }
        else
        {
            textElement.text = "N/A";
        }
    }

    /// <summary>
    /// Set every text in ammoUi to N/A
    /// </summary>
    private void SetEmptyAll()
    {
        if (centerWeaponText != null) centerWeaponText.text = "N/A";
        if (rightWeaponText != null) rightWeaponText.text = "N/A";
        if (bottomWeaponText != null) bottomWeaponText.text = "N/A";
        if (leftWeaponText != null) leftWeaponText.text = "N/A";
    }
}
