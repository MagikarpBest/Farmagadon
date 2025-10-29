using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Image leftWeaponImage;// 4th slot 
    [SerializeField] private Image centerWeaponImage;
    [SerializeField] private Image rightWeaponImage;
    [SerializeField] private Image bottomWeaponImage;     
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Animation Settings")]
    [SerializeField] private float rotateDuration = 0.25f;
    [SerializeField] private Ease rotateEase = Ease.OutQuad;

    private bool canSwap;
    // Cached original positions
    private Vector3 leftPosition;
    private Vector3 centerPosition;
    private Vector3 rightPosition;
    private Vector3 bottomPosition;

    // Store the initial size of each slot
    private Vector2 leftOriginalSize;
    private Vector2 centerOriginalSize;
    private Vector2 rightOriginalSize;
    private Vector2 bottomOriginalSize;

    private void OnEnable()
    {
        if (weaponInventory != null)
        {
            weaponInventory.OnWeaponChanged += UpdateUI;

        }
    }
    private void OnDisable()
    {
        if (weaponInventory != null)
        {
            weaponInventory.OnWeaponChanged -= UpdateUI;
        }
    }

    private void Start()
    {
        // Save original position for animation references
        leftPosition = leftWeaponImage.rectTransform.anchoredPosition;
        centerPosition = centerWeaponImage.rectTransform.anchoredPosition;
        rightPosition = rightWeaponImage.rectTransform.anchoredPosition;
        bottomPosition = bottomWeaponImage.rectTransform.anchoredPosition;

        leftOriginalSize = leftWeaponImage.rectTransform.sizeDelta;
        centerOriginalSize = centerWeaponImage.rectTransform.sizeDelta;
        rightOriginalSize = rightWeaponImage.rectTransform.sizeDelta;
        bottomOriginalSize = bottomWeaponImage.rectTransform.sizeDelta;

        if (weaponInventory != null)
        {
            InitializeWeaponImages();
        }
    }

    private void InitializeWeaponImages()
    {
        // Always show all slots (up to max)
        int totalSlots = weaponInventory.UnlockedSlotCount; // using maxSlot for full UI display
        List<WeaponSlot> allSlots = new();

        for (int i = 0; i < totalSlots; i++)
        {
            allSlots.Add(weaponInventory.GetWeaponSlot(i)); // may include nulls (locked or empty)
        }

        if (allSlots.Count == 0)
        {
            SetEmptyAll();
            return;
        }

        int currentIndex = weaponInventory.GetCurrentWeaponIndex();

        // --- Wrap-around neighbors ---
        WeaponSlot centerSlot = allSlots[currentIndex];
        WeaponSlot rightSlot = allSlots[(currentIndex + 1) % totalSlots];
        WeaponSlot bottomSlot = allSlots[(currentIndex + 2) % totalSlots];
        WeaponSlot leftSlot = allSlots[(currentIndex + 3) % totalSlots];

        // Assign Images
        SetImage(centerWeaponImage, centerSlot);
        SetImage(rightWeaponImage, rightSlot);
        SetImage(bottomWeaponImage, bottomSlot);
        SetImage(leftWeaponImage, leftSlot);
    }
    private void UpdateUI(WeaponSlot currentSlot,WeaponSwitchDirection direction)
    {
        if (weaponInventory == null || currentSlot == null)  
        {
            Debug.LogWarning("[WeaponUI] Missing inventory or current slot.");
            return;
        }

        // Animate depending on switch direction
        if (direction == WeaponSwitchDirection.Next)
        {
            RotateAnimation(true);
        }
        else if (direction == WeaponSwitchDirection.Previous)
        {
            RotateAnimation(false);
        }

    }

    private void RotateAnimation(bool clockwise)
    {
        var left = leftWeaponImage.rectTransform;
        var center = centerWeaponImage.rectTransform;
        var right = rightWeaponImage.rectTransform;
        var bottom = bottomWeaponImage.rectTransform;

        Sequence seq = DOTween.Sequence();

        if (clockwise)
        {
            // Anti Clockwise, When E pressed, move icon to left
            // Move positions
            seq.Join(left.DOAnchorPos(bottomPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOAnchorPos(leftPosition, rotateDuration).SetEase(rotateEase));

            // Tween scales
            seq.Join(left.DOSizeDelta(bottomOriginalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(rightOriginalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(centerOriginalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOSizeDelta(leftOriginalSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                Image temp = leftWeaponImage;
                leftWeaponImage = centerWeaponImage;
                centerWeaponImage = rightWeaponImage;
                rightWeaponImage = bottomWeaponImage;
                bottomWeaponImage = temp;
                ResetPositions();
                ResetScales();
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
            seq.Join(left.DOSizeDelta(centerOriginalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOSizeDelta(rightOriginalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(bottomOriginalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(leftOriginalSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                Image temp = leftWeaponImage;
                leftWeaponImage = bottomWeaponImage;
                bottomWeaponImage = rightWeaponImage;
                rightWeaponImage = centerWeaponImage;
                centerWeaponImage = temp;
                ResetPositions();
                ResetScales();
            });
        }
    }

    private void ResetPositions()
    {
        leftWeaponImage.rectTransform.anchoredPosition = leftPosition;
        centerWeaponImage.rectTransform.anchoredPosition = centerPosition;
        rightWeaponImage.rectTransform.anchoredPosition = rightPosition;
        bottomWeaponImage.rectTransform.anchoredPosition = bottomPosition;
    }

    private void ResetScales()
    {
        leftWeaponImage.rectTransform.sizeDelta = leftOriginalSize;
        centerWeaponImage.rectTransform.sizeDelta = centerOriginalSize;
        rightWeaponImage.rectTransform.sizeDelta = rightOriginalSize;
        bottomWeaponImage.rectTransform.sizeDelta = bottomOriginalSize;
    }

    private void SetImage(Image image, WeaponSlot slot)
    {
        if (image == null)
        {
            return;
        }

        if (slot != null && slot.weaponData != null & slot.weaponData.weaponSprite != null)
        {
            image.sprite = slot.weaponData.weaponSprite;
        }
        else
        {
            image.sprite = emptySlotSprite;
        }
    }

    private void SetEmptyAll()
    {
        SetImage(leftWeaponImage, null);
        SetImage(centerWeaponImage, null);
        SetImage(rightWeaponImage, null);
        SetImage(bottomWeaponImage, null);
    }

}
