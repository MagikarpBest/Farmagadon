using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanelUpdater : MonoBehaviour
{
    private AmmoData ammoData;
    public AmmoData AmmoData {  set { ammoData = value; } }
    private AmmoInventory ammoInventory;
    public AmmoInventory AmmoInventory { set { ammoInventory = value; } }
    [SerializeField] private Image ammoImage;
    [SerializeField] private AudioClip updateAudio;
    private TextMeshProUGUI ammoText;


    private bool scaleCD = false;
    private bool firstRun = true;

    public Transform AmmoImage => ammoImage.transform;

    private void Awake()
    {
        
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetImage(Sprite sprite)
    {
        ammoImage.sprite = sprite;
    }

    public void UpdateSelf()
    {
        
        
        ammoText.text = "X " + ammoInventory.GetAmmoCount(ammoData);
        if (firstRun) { firstRun = false; return; }
        AudioService.AudioManager.BufferPlayOneShot(updateAudio);
        if (!scaleCD)
        {
            StartCoroutine(BounceText());
        }

    }

    private IEnumerator BounceText()
    {
        scaleCD = true;
        Tween punchTween = ammoText.transform.DOPunchScale(new Vector3(0, 1, 0), 0.2f, 1);
        yield return punchTween.WaitForCompletion();
        scaleCD = false;
    }
}
