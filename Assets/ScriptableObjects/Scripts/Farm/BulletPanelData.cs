using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BulletPanelData", menuName = "Crops/BulletPanelData")]
public class BulletPanelData : ScriptableObject
{
    public Image bulletPanel;
    public string ammoInvKey;
    public string weaponID;
}
