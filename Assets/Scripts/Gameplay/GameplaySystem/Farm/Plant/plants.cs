using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class plants : MonoBehaviour
{
    [SerializeField] FlashEffect flashEffect;
    [SerializeField] AudioClip farmedAudio;


    private AmmoData plantAmmoData;
    private int dropAmount;
    private int posX;
    private int posY;

    public AmmoData PlantAmmoData { set { plantAmmoData = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }
    public int PosX { set { posX = value; } }
    public int PosY { set { posY = value; } }


    public delegate void Destroyed(int posX, int posY, Vector3 endPos, AmmoData data, BulletPanelUpdater updater, float duration=1.0f, int dropAmount = 0);
    public Destroyed OnDestroyed;
    public delegate void Farmed(AmmoData plantName, int dropAmount);
    public Farmed OnFarmed;
    
    public void DestroySelf()
    {
        
        //OnFarmed?.Invoke(plantAmmoData, dropAmount);
        flashEffect.CallDamageFlash();
        Sequence plantShrink = DOTween.Sequence();
        plantShrink.Prepend(transform.DOPunchPosition(new Vector3(0.1f, 0.0f, 0.0f), 0.5f));
        plantShrink.Prepend(GetComponentInChildren<SpriteRenderer>().DOFade(0, 0.5f));
        plantShrink.Prepend(transform.DOScale(0, 0.5f));
        AudioService.AudioManager.BufferPlayOneShot(farmedAudio, 0.5f);


        Image panel = BulletPanelHandler.GetBulletPanel(plantAmmoData);

        // Hard check for destroyed UI object (Unity ghost-null problem)
        if (ReferenceEquals(panel, null) || ReferenceEquals(panel?.gameObject, null))
        {
            Debug.LogWarning("BulletPanel destroyed or missing, skipping UI animation.");

            // Still invoke event but with null panel/updater
            OnDestroyed?.Invoke(posX, posY, Vector3.zero, plantAmmoData, null, 1.0f, dropAmount);

            StartCoroutine(destroyAfter(plantShrink.Duration()));
            return;
        }

        // Safe get component
        BulletPanelUpdater updater = panel.GetComponent<BulletPanelUpdater>();

        if (ReferenceEquals(updater, null))
        {
            Debug.LogWarning("BulletPanelUpdater destroyed or missing.");

            OnDestroyed?.Invoke(posX, posY, Vector3.zero, plantAmmoData, null, 1.0f, dropAmount);

            StartCoroutine(destroyAfter(plantShrink.Duration()));
            return;
        }

        // SAFE animation position
        Vector3 panelPos = updater.AmmoImage.position;

        // SAFE invoke
        OnDestroyed?.Invoke(posX, posY, panelPos, plantAmmoData, updater, 1.0f, dropAmount);
    }

    private IEnumerator destroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        Destroy(gameObject);
    }
}
