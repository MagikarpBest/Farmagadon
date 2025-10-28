using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletPanelUpdater : MonoBehaviour
{
    private AmmoData ammoData;
    public AmmoData AmmoData {  set { ammoData = value; } }
    private AmmoInventory ammoInventory;
    public AmmoInventory AmmoInventory { set { ammoInventory = value; } }
    private Image ammoImage;
    private TextMeshProUGUI ammoText;
    

    private void Awake()
    {
        ammoImage = GetComponentsInChildren<Image>()[2];
        
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetImage(Sprite sprite)
    {
        ammoImage.sprite = sprite;
    }
    public void UpdateSelf()
    {
        ammoText.text = "X " + ammoInventory.GetAmmoCount(ammoData);
    }
}
