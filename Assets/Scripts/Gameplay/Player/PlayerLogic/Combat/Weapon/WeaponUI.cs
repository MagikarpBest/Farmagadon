using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Handles the visual representation of the player's weapon wheel UI.
/// Displays currently equipped and unlocked weapon slots, 
/// and animates transitions when switching weapons.
/// </summary>
public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Image centerWeaponImage;            // 1st slot 
    [SerializeField] private Image rightWeaponImage;             // 2nd slot 
    [SerializeField] private Image bottomWeaponImage;            // 3rd slot 
    [SerializeField] private Image leftWeaponImage;              // 4th slot 
    [SerializeField] private Sprite emptySlotSprite;             // Sprite for empty or locked slots

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

        // Prevent any stray tween movement
        DOTween.KillAll();

        // Save size
        originalSize = leftWeaponImage.rectTransform.sizeDelta;
        centerBigSize = centerWeaponImage.rectTransform.sizeDelta;

        if (weaponInventory != null)
        {
            InitializeWeaponImages();
        }
    }

    /// <summary>
    /// Initializes weapon slot icons based on unlocked weapon slots.
    /// Always displays 4 UI slots (center, right, bottom, left),
    /// but only fills unlocked ones with real weapon icons.
    /// </summary>
    private void InitializeWeaponImages()
    {
        // Always show all slots (up to max)
        int unlocked = weaponInventory.UnlockedSlotCount;
        List<WeaponSlot> allSlots = new();

        // Add all unlocked weapon slots
        for (int i = 0; i < unlocked; i++)
        {
            allSlots.Add(weaponInventory.GetWeaponSlotOfSpecificIndex(i)); // may include nulls (locked or empty)
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

        // Assign Images
        SetImage(centerWeaponImage, centerSlot);
        SetImage(rightWeaponImage, rightSlot);
        SetImage(bottomWeaponImage, bottomSlot);
        SetImage(leftWeaponImage, leftSlot);
    }

    /// <summary>
    /// Called when the active weapon changes.
    /// Triggers the appropriate rotation animation based on unlocked weapon count.
    /// </summary>
    private void UpdateUI(WeaponSlot currentSlot,WeaponSwitchDirection direction)
    {
        if (weaponInventory == null || currentSlot == null)  
        {
            Debug.LogWarning("[WeaponUI] Missing inventory or current slot.");
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
        var center = centerWeaponImage.rectTransform;
        var right = rightWeaponImage.rectTransform;

        Sequence seq = DOTween.Sequence();

        seq.Join(center.DOAnchorPos(rightPosition, rotateDuration).SetEase(rotateEase));
        seq.Join(right.DOAnchorPos(centerPosition, rotateDuration).SetEase(rotateEase));

        seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
        seq.Join(right.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));

        seq.OnComplete(() =>
        {
            // Swap references
            Image temp = centerWeaponImage;
            centerWeaponImage = rightWeaponImage;
            rightWeaponImage = temp;

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
        var center = centerWeaponImage.rectTransform;
        var right = rightWeaponImage.rectTransform;
        var bottom = bottomWeaponImage.rectTransform;

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
                Image temp = centerWeaponImage;
                centerWeaponImage = rightWeaponImage;
                rightWeaponImage = bottomWeaponImage;
                bottomWeaponImage = temp;
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
                Image temp = centerWeaponImage;
                centerWeaponImage = bottomWeaponImage;
                bottomWeaponImage = rightWeaponImage;
                rightWeaponImage = temp;
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

            // Tween sizes
            seq.Join(left.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(bottom.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));
            seq.Join(right.DOSizeDelta(centerBigSize, rotateDuration).SetEase(rotateEase));
            seq.Join(center.DOSizeDelta(originalSize, rotateDuration).SetEase(rotateEase));

            seq.OnComplete(() =>
            {
                Image temp = leftWeaponImage;
                leftWeaponImage = centerWeaponImage;
                centerWeaponImage = rightWeaponImage;
                rightWeaponImage = bottomWeaponImage;
                bottomWeaponImage = temp;
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
                Image temp = leftWeaponImage;
                leftWeaponImage = bottomWeaponImage;
                bottomWeaponImage = rightWeaponImage;
                rightWeaponImage = centerWeaponImage;
                centerWeaponImage = temp;
                ResetPositions();
                ResetSizes();
            });
        }
    }

    /// <summary>
    /// Resets all weapon image positions to their default layout.
    /// </summary>
    private void ResetPositions()
    {
        leftWeaponImage.rectTransform.anchoredPosition = leftPosition;
        centerWeaponImage.rectTransform.anchoredPosition = centerPosition;
        rightWeaponImage.rectTransform.anchoredPosition = rightPosition;
        bottomWeaponImage.rectTransform.anchoredPosition = bottomPosition;
    }

    /// <summary>
    /// Resets all weapon image sizes to their original values.
    /// </summary>
    private void ResetSizes()
    {
        leftWeaponImage.rectTransform.sizeDelta = originalSize;
        centerWeaponImage.rectTransform.sizeDelta = centerBigSize;
        rightWeaponImage.rectTransform.sizeDelta = originalSize;
        bottomWeaponImage.rectTransform.sizeDelta = originalSize;
    }

    /// <summary>
    /// Sets the given image sprite to the weapon icon or empty slot if null.
    /// </summary>
    public void SetImage(Image image, WeaponSlot slot)
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

    /// <summary>
    /// Clears all weapon images to the empty slot sprite.
    /// </summary>
    private void SetEmptyAll()
    {
        SetImage(leftWeaponImage, null);
        SetImage(centerWeaponImage, null);
        SetImage(rightWeaponImage, null);
        SetImage(bottomWeaponImage, null);
    }
}
