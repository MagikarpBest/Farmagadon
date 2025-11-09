using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class plants : MonoBehaviour
{
    [SerializeField] FlashEffect flashEffect;


    private AmmoData plantAmmoData;
    private int dropAmount;
    private int posX;
    private int posY;

    public AmmoData PlantAmmoData { set { plantAmmoData = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }
    public int PosX { set { posX = value; } }
    public int PosY { set { posY = value; } }


    public delegate void Destroyed(int posX, int posY, Vector3 endPos, AmmoData data, float duration=1.0f);
    public Destroyed OnDestroyed;
    public delegate void Farmed(AmmoData plantName, int dropAmount);
    public Farmed OnFarmed;
    
    public void DestroySelf()
    {
        
        OnFarmed?.Invoke(plantAmmoData, dropAmount);
        transform.DOPunchPosition(new Vector3(0.1f, 0.0f, 0.0f), 0.5f);
        flashEffect.CallDamageFlash();
        GetComponentInChildren<SpriteRenderer>().DOFade(0, 0.5f);
        transform.DOScale(0, 0.5f);

        Image panel = BulletPanelHandler.GetBulletPanel(plantAmmoData);
        Vector3 panelPos = panel.transform.position;
        
        OnDestroyed?.Invoke(posX, posY, panelPos, plantAmmoData, 1.0f);
        StartCoroutine(destroyAfter(0.5f));
    }

    private IEnumerator destroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
